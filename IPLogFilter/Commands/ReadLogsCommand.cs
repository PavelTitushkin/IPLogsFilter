using IPLogFilter.Commands.Contracts;
using IPLogFilter.Models;
using System.Net;

namespace IPLogFilter.Commands
{
    public class ReadLogsCommand : ILogCommand
    {
        private readonly CommandLogOptions _options;

        public ReadLogsCommand(CommandLogOptions options)
        {
            _options = options;
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(_options.FileLog))
            {
                throw new FileNotFoundException("Отсутствует путь к файлу!");
            }
            _options.LogRecords = File.ReadAllLines(_options.FileLog)
                .Select(l =>
                {
                    
                    var index = l.Trim().IndexOf(":");
                    IPAddress.TryParse(l.Substring(0, index), out IPAddress ipAddress);
                    var dateVisit = l.Substring(index + 1, l.Length - index - 1);
                    DateTime.TryParse(dateVisit, out DateTime result);

                    return new LogRecord { IPAddress = ipAddress, TimeVisit = result };
                })
                .ToList();
        }
    }
}
