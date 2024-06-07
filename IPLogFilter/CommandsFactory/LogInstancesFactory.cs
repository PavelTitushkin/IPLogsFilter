using IPLogFilter.Commands;
using IPLogFilter.Commands.Contracts;
using IPLogFilter.Commands.Executers;
using IPLogFilter.Models;

namespace IPLogFilter.CommandsFactory
{
    public class LogInstancesFactory
    {
        public static ICommandLogsExecuter CreateExecuter(ILogCommand[] commands)
        {
            return new CommandLogsExecutor(commands);
        }

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
