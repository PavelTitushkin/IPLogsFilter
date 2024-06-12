using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPLogsFilter.Bussines.ReadLogsHostedService.Services
{
    public class IPLogsBackgroundReader : BackgroundService
    {
        private IServiceProvider _services;
        public AppSettings AppSettings;

        public IPLogsBackgroundReader(IOptionsMonitor<AppSettings> options, IServiceProvider services)
        {
            _services = services;
            AppSettings = options.CurrentValue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var logFiles = Directory.GetFiles(AppSettings.PathToFolderWithLogs, "*.log", SearchOption.TopDirectoryOnly)
                        .OrderBy(f=>f)
                        .Take(AppSettings.FilesToProcessPerIteration)
                        .ToList();
                    foreach (var logFilePath in logFiles)
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }
                        using var scope = _services.CreateScope();
                        var logService = scope.ServiceProvider.GetRequiredService<ILogFilterService>();
                        var repositoryService = scope.ServiceProvider.GetRequiredService<IIPLogsFilterRepository>();

                        if (await repositoryService.IsProcessedLogFileAsync(logFilePath, stoppingToken))
                        {
                            continue;
                        }

                        await logService.ReadLogsFromFileAsync(logFilePath, stoppingToken);                        
                    }

                    await Task.Delay(TimeSpan.FromSeconds(AppSettings.TimeToCheckForNewRawFiles), stoppingToken);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
