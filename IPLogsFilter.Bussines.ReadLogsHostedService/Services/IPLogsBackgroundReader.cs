using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Configuration;
using IPLogsFilter.Bussines.ReadLogsHostedService.Contracts;
using IPLogsFilter.Bussines.ReadLogsHostedService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IPLogsFilter.Bussines.ReadLogsHostedService.Services
{
    public class IPLogsBackgroundReader : BackgroundService, ILogBackgroundReader
    {
        private IServiceProvider _services;
        public AppSettings AppSettings;
        private DateTime? _lastRunStartTime;
        private DateTime? _lastRunEndTime;
        private int _processedFilesCount;
        private int _totalFilesToProcess;
        private TaskCompletionSource<bool> _completionSource;
        private bool _processRemaining;

        public IPLogsBackgroundReader(IOptionsMonitor<AppSettings> options, IServiceProvider services)
        {
            _services = services;
            AppSettings = options.CurrentValue;
            _completionSource = new TaskCompletionSource<bool>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _lastRunStartTime = DateTime.UtcNow;
                _processedFilesCount = 0;
                try
                {
                    var logFiles = Directory.GetFiles(AppSettings.PathToFolderWithLogs, "*.log", SearchOption.TopDirectoryOnly)
                        .OrderBy(f => f)
                        .Take(AppSettings.FilesToProcessPerIteration)
                        .ToList();

                    _totalFilesToProcess = logFiles.Count;

                    foreach (var logFilePath in logFiles)
                    {
                        if (stoppingToken.IsCancellationRequested && !_processRemaining)
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
                        _processedFilesCount++;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(AppSettings.TimeToCheckForNewRawFiles), stoppingToken);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    _lastRunEndTime = DateTime.UtcNow;
                }

                await _completionSource.Task;
            }
        }

        public HandlerStatus GetStatusHandler()
        {
            return new HandlerStatus
            {
                ProcessedFilesCount = _processedFilesCount,
                TotalFilesToProcess = _totalFilesToProcess,
                LastRunStartTime = _lastRunStartTime,
                LastRunEndTime = _lastRunEndTime,
                IsRunning = !_completionSource.Task.IsCompleted
            };
        }

        public void StopHandler(bool processRemaining)
        {
            _processRemaining = processRemaining;
            _completionSource.TrySetResult(true);
        }


        public void StartHandler(CancellationToken cancellationToken)
        {

            if (_completionSource.Task.IsCompleted)
            {
                _completionSource = new TaskCompletionSource<bool>();
            }

            ExecuteAsync(cancellationToken);
        }
    }
}
