using IPLogFilter.Commands.Executers;
using IPLogFilter.CommandsFactory;
using IPLogFilter.Models;
using IPLogFilter.ParsersLogs;
using Microsoft.Extensions.Configuration;

namespace IPLogFilter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
                IConfiguration config = builder.Build();

                var parser = new CommandLineArgsParser();
                var options = parser.ParseArgs(args);

                options = new CommandLogOptions();
                options.FileLog = config["FileLog"];
                options.FileOutput = config["FileOutput"];
                options.AddressStart = config["AddressStart"];
                options.AddressMask = config["AddressMask"];
                DateTime.TryParse(config["TimeStart"], out DateTime timeStart);
                DateTime.TryParse(config["TimeEnd"], out DateTime timeEnd);
                options.TimeStart = timeStart;
                options.TimeEnd = timeEnd;

                var commands = LogCommandFactory.CreateCommands(options);
                var executor = new CommandLogsExecutor(commands);
                executor.ExecuteCommands();
            }
            catch (FileNotFoundException e)
            {
                var parser = new CommandLineArgsParser();
                var options = parser.ParseArgs(args);

                var commands = LogCommandFactory.CreateCommands(options);
                var executor = new CommandLogsExecutor(commands);
                executor.ExecuteCommands();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }        
        }
    }
}
