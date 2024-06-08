using IPLogFilter.CommandsFactory;
using IPLogFilter.Models;
using IPLogFilter.ParsersLogs;
using IPLogsFilter.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
                var services = new ServiceCollection();
                services.AddDbContext<IPLogsFilterContext>(
                   optionsBuilder =>
                   {
                       optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"),
                           sqlOptionsBuilder =>
                           {
                               sqlOptionsBuilder.EnableRetryOnFailure();
                           })
                       .UseSnakeCaseNamingConvention();
                   });

                var serviceProvider = services.BuildServiceProvider();
                var context = serviceProvider.GetService<IPLogsFilterContext>();
                var entity = context.LogRecords.Where(x=>x.Id == 1).FirstOrDefault();
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

                var connectionString = config.GetConnectionString("DefaultConnection");
                
                // Повторение кода, можно вынести в отдельный метод
                var commands = LogInstancesFactory.CreateCommands(options);
                var executor = LogInstancesFactory.CreateExecuter(commands);
                executor.ExecuteCommands();
            }
            catch (FileNotFoundException e)
            {
                var parser = new CommandLineArgsParser();
                var options = parser.ParseArgs(args);

                // Повторение кода, можно вынести в отдельный метод
                var commands = LogInstancesFactory.CreateCommands(options);
                var executor = LogInstancesFactory.CreateExecuter(commands);
                executor.ExecuteCommands();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
