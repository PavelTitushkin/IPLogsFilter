using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLogsFilter.Bussines.ReadLogsHostedService.Services
{
    public class MinioIpLogsBackgroundReader : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMinioClient _minioClient;
        //private int countReadedFiles = 0;
        public AppSettingsMinio AppSettingsMinio { get; set; }
        public MinioIpLogsBackgroundReader(IServiceProvider serviceProvider, IOptionsMonitor<AppSettingsMinio> options, IMinioClient minioClient)
        {
            _serviceProvider = serviceProvider;
            AppSettingsMinio = options.CurrentValue;
            _minioClient = minioClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var logFiles = await GetLogFilesFromMinioAsync(stoppingToken);

                    foreach (var logFilePath in logFiles
                        .OrderBy(f => f)
                        //.Skip(countReadedFiles)
                        .Take(AppSettingsMinio.FilesToProcessPerIteration)
                        .ToList())
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }

                        using var scope = _serviceProvider.CreateScope();
                        var logService = scope.ServiceProvider.GetRequiredService<ILogFilterService>();
                        var repositoryService = scope.ServiceProvider.GetRequiredService<IIPLogsFilterRepository>();

                        if (await repositoryService.IsProcessedLogFileAsync(logFilePath, stoppingToken))
                        {
                            continue;
                        }

                        await logService.ReadLogsFromFileAsync(logFilePath, stoppingToken);
                    }

                    //countReadedFiles += AppSettingsMinio.FilesToProcessPerIteration;
                    await Task.Delay(TimeSpan.FromSeconds(AppSettingsMinio.TimeToCheckForNewRawFiles), stoppingToken);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private async Task<List<string>> GetLogFilesFromMinioAsync(CancellationToken stoppingToken)
        {
            var logFiles = new List<string>();

            var args = new ListObjectsArgs()
                .WithBucket(AppSettingsMinio.BucketName);

            var logs = _minioClient.ListObjectsAsync(args, stoppingToken);
            foreach (var log in logs)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                if (log.Key.EndsWith(".log"))
                {
                    var fileName = Path.GetFileName(log.Key);
                    var localFilePath = Path.Combine(AppSettingsMinio.PathFolderToSaveFile, fileName);

                    var getObjectArgs = new GetObjectArgs()
                        .WithBucket(AppSettingsMinio.BucketName)
                        .WithObject(log.Key)
                        .WithFile(localFilePath);
                        //.WithCallbackStream(async stream =>
                        //    {
                        //        using var fileStream = File.Create(localFilePath);
                        //        await stream.CopyToAsync(fileStream, stoppingToken);
                        //    });

                    await _minioClient.GetObjectAsync(getObjectArgs, stoppingToken);
                    logFiles.Add(localFilePath);
                }
            }

            return logFiles;
        }
    }
}
