using IPLogsFilter.Abstractions.Entities;
using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using System.Net;
using System.Net.Sockets;

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

        public void WriteLogsToDb(List<FiltredLogs?> filterLogs)
        {
            _repository.WriteLogs(filterLogs);
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
    }
}
