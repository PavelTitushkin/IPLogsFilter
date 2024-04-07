using IPLogFilter.Commands.Contracts;
using IPLogFilter.Models;

namespace IPLogFilter.Commands
{
    public class WriteLogsResultsCommand : ILogCommand
    {
        private readonly CommandLogOptions _options;

        public WriteLogsResultsCommand(CommandLogOptions options)
        {
            _options = options;
        }

        public void Execute()
        {
            if (_options.FileOutput == null)
            {
                throw new ArgumentNullException("Не указан путь сохранения результатов!");
            }
            using (var writer = new StreamWriter(_options.FileOutput))
            {
                foreach (var kvp in _options.CountVisits)
                {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }
    }
}
