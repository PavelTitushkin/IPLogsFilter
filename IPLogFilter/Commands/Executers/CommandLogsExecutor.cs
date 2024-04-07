using IPLogFilter.Commands.Contracts;

namespace IPLogFilter.Commands.Executers
{
    public class CommandLogsExecutor
    {
        private readonly ILogCommand[] _commands;

        public CommandLogsExecutor(ILogCommand[] commands)
        {
            _commands = commands;
        }

        public void ExecuteCommands()
        {
            try
            {
                foreach (var command in _commands)
                {
                    command.Execute();
                }
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
