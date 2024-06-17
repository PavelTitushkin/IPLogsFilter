using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebLogsProvider.Contracts;
using WebLogsProvider.Models;
using WebLogsProvider.Models.ModelsConfig;

namespace WebLogsProvider.Services
{
    public class WebLogProccessor : IWebLogProccessor
    {
        public AppSettings AppSettings { get; set; }

        public WebLogProccessor(IOptionsMonitor<AppSettings> options)
        {
            AppSettings = options.CurrentValue;
        }

        public async Task<IEnumerable<LogFile?>> GetLogsByTimeAsync(DateTime? time, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(AppSettings.PathToFolderWithLogs))
            {
                throw new DirectoryNotFoundException($"Directory '{AppSettings.PathToFolderWithLogs}' not found.");
            }


            var files = Directory.GetFiles(AppSettings.PathToFolderWithLogs, "*.accesslog");

            var logFiles = new List<LogFile>();

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var creationTime = await Task.Run(() => File.GetCreationTimeUtc(file), cancellationToken);

                if (creationTime > time)
                {
                    var fileInfo = new FileInfo(file);
                    var logFile = new LogFile
                    {
                        Name = fileInfo.Name,
                        Size = fileInfo.Length
                    };

                    logFiles.Add(logFile);
                }
            }

            return logFiles;
        }

        public LogFile? GetLog(string filename)
        {
            if (!Directory.Exists(AppSettings.PathToFolderWithLogs))
            {
                throw new DirectoryNotFoundException($"Directory '{AppSettings.PathToFolderWithLogs}' not found.");
            }

            var filePath = Path.Combine(AppSettings.PathToFolderWithLogs, filename + ".accesslog");

            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);

                var logFile = new LogFile
                {
                    Name = fileInfo.Name,
                    Size = fileInfo.Length
                };

                return logFile;
            }

            return null;
        }
    }
}
