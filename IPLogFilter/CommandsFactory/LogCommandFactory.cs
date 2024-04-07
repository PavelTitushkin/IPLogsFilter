using IPLogFilter.Commands;
using IPLogFilter.Commands.Contracts;
using IPLogFilter.Models;

namespace IPLogFilter.CommandsFactory
{
    public class LogCommandFactory
    {
        public static ILogCommand[] CreateCommands(CommandLogOptions options)
        {
            return
            [
                new ReadLogsCommand(options),
                new FilterLogsCommand(options),
                new CountVisitsCommand(options),
                new WriteLogsResultsCommand(options)
            ];
        }
    }
}
