using IPLogFilter.Commands.Contracts;
using IPLogFilter.Models;

namespace IPLogFilter.Commands
{
    public class CountVisitsCommand : ILogCommand
    {
        private readonly CommandLogOptions _options;

        public CountVisitsCommand(CommandLogOptions options)
        {
            _options = options;
        }

        public void Execute()
        {
            if (_options.LogRecords != null && _options.LogRecords.Any())
            {
                foreach (var log in _options.LogRecords)
                {
                    if (_options.CountVisits.ContainsKey(log.IPAddress))
                    {
                        _options.CountVisits[log.IPAddress]++;
                    }
                    else
                    {
                        _options.CountVisits[log.IPAddress] = 1;
                    }
                }
            }
        }
    }
}
