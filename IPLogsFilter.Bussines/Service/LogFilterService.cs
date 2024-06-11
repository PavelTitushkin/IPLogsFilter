using IPLogsFilter.Abstractions.Entities;
using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace IPLogsFilter.Bussines.Service
{
    public class LogFilterService : ILogFilterService
    {
        private readonly IIPLogsFilterRepository _repository;

        public LogFilterService(IIPLogsFilterRepository repository)
        {
            _repository = repository;
        }

        public List<LogRecord> GetLogRecords()
        {
            return _repository.ReadLogs();
        }

        public List<LogRecord> FilterLogs(List<LogRecord> logs, string? addressStart, string? addressMask, DateTime timeStart, DateTime timeEnd)
        {
            if (logs == null || !logs.Any())
            {
                throw new ArgumentNullException("Отсутствуют логи!");
            }
            if (!string.IsNullOrEmpty(addressStart))
            {
                IPAddress.TryParse(addressStart, out IPAddress startAdress);
                if (!string.IsNullOrEmpty(addressMask))
                {
                    IPAddress.TryParse(addressMask, out IPAddress maskAddress);
                    logs = logs.Where(log => IsIpAddressInRangeWithMask(log.ClientIpAddress, startAdress, maskAddress)).ToList();
                }
                else
                {
                    logs = logs.Where(log => IsIpAddressInRange(log.ClientIpAddress, startAdress)).ToList();
                }
            }

            return logs = logs.Where(log => log.RequestTime >= timeStart && log.RequestTime <= timeEnd).ToList();
        }

        public List<FiltredLogs> CountingIPVisits(List<LogRecord> logs)
        {
            var filtredLogs = new List<FiltredLogs>();
            var countVisits = new Dictionary<IPAddress, int>();
            if (logs != null && logs.Any())
            {
                foreach (var log in logs)
                {
                    if (countVisits.ContainsKey(log.ClientIpAddress))
                    {
                        countVisits[log.ClientIpAddress]++;
                    }
                    else
                    {
                        countVisits[log.ClientIpAddress] = 1;
                    }
                }
            }

            foreach (var item in countVisits)
            {
                filtredLogs.Add(new FiltredLogs { LogRecord = item.Key.ToString(), CountLogRecords = item.Value });
            }

            return filtredLogs;
        }

        public void WriteFiltredLogsToDb(List<FiltredLogs?> filterLogs)
        {
            _repository.WriteFiltredLogs(filterLogs);
        }

        public async Task ReadLogsFromFileAsync(string logFilePath, CancellationToken cancellationToken)
        {
            var lastLine = await _repository.GetLastUnprocessedLineAsync(logFilePath, cancellationToken);
            var logRecords = new List<LogRecord>();
            using(var reader = new StreamReader(logFilePath))
            {
                var line = string.Empty;
                var currentLine = 0;
                while (currentLine < lastLine && (line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    currentLine++;
                }
                while((line  = await reader.ReadLineAsync(cancellationToken)) != null)
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    try
                    {
                        var log = ParseLogRecord(line);
                        await _repository.LoggingLogAndStateFromFileToDatabaseAsync(log, logFilePath, ++currentLine, cancellationToken);
                        //logRecords.Add(log);            
                        //сохранили состояние и записи в БД с помощью транзакции.
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                await _repository.CompletingLogReadingAsync(logFilePath, cancellationToken);
                //await _repository.LoggingLogsFromFileToDatabaseAsync(logRecords, cancellationToken);
            }
        }


        private bool IsIpAddressInRangeWithMask(IPAddress ipAddress, IPAddress startAddress, IPAddress addressMask)
        {
            var startAddressBytes = startAddress.GetAddressBytes();
            Array.Reverse(startAddressBytes);
            var addressMaskBytes = addressMask.GetAddressBytes();
            Array.Reverse(addressMaskBytes);
            var ipAddressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(ipAddressBytes);

            if (startAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var ipAddressToInt = BitConverter.ToUInt32(ipAddressBytes);

                return BitConverter.ToUInt32(startAddressBytes, 0) <= ipAddressToInt && ipAddressToInt <= BitConverter.ToUInt32(addressMaskBytes, 0);
            }

            return false;
        }

        private bool IsIpAddressInRange(IPAddress ipAddress, IPAddress startAddress)
        {
            var startAddressBytes = startAddress.GetAddressBytes();
            Array.Reverse(startAddressBytes);
            var ipAddressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(ipAddressBytes);

            if (startAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var ipAddressToInt = BitConverter.ToUInt32(ipAddressBytes);

                return BitConverter.ToUInt32(startAddressBytes, 0) <= ipAddressToInt;
            }

            return false;
        }

        private LogRecord ParseLogRecord(string line)
        {
            var pattern = @"{RequestTime:(?<RequestTime>[^,]+),ApplicationName:(?<ApplicationName>[^,]*),Stage:(?<Stage>[^,]*),ClientIpAddress:(?<ClientIpAddress>[^,]+),ClientName:(?<ClientName>[^,]*),ClientVersion:(?<ClientVersion>[^,]*),Path:(?<Path>[^,]+),Method:(?<Method>[^,]+),StatusCode:(?<StatusCode>[^,]*),StatusMesage:(?<StatusMesage>[^,]*),ContentType:(?<ContentType>[^,]*),ContentLength:(?<ContentLength>[^,]*),ExecutionTime:(?<ExecutionTime>[^,]*),MemroyUsage:(?<MemroyUsage>[^,]*),}";

            var match = Regex.Match(line, pattern);
            if (!match.Success)
            {
                throw new FormatException("Не верный формат лога!");
            }

            var log = new LogRecord
            {
                RequestTime = DateTime.SpecifyKind(DateTime.Parse(match.Groups["RequestTime"].Value), DateTimeKind.Utc),
                ApplicationName = match.Groups["ApplicationName"].Value,
                Stage = match.Groups["Stage"].Value,
                ClientIpAddress = IPAddress.Parse(match.Groups["ClientIpAddress"].Value),
                ClientName = match.Groups["ClientName"].Value,
                ClientVersion = match.Groups["ClientVersion"].Value,
                Path = match.Groups["Path"].Value,
                Method = match.Groups["Method"].Value,
                StatusCode = match.Groups["StatusCode"].Value,
                StatusMesage = match.Groups["StatusMesage"].Value,
                ContentType = match.Groups["ContentType"].Value,
                ContentLength = match.Groups["ContentLength"].Value,
                ExecutionTime = TimeSpan.TryParse(match.Groups["ExecutionTime"].Value, out var executionTime) ? executionTime : (TimeSpan?)null,
                MemroyUsage = int.TryParse(match.Groups["MemroyUsage"].Value, out var memoryUsage) ? memoryUsage : (int?)null
            };

            return log;
        }
    }
}
