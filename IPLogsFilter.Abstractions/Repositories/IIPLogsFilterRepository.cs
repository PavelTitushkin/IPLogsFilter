using IPLogsFilter.Abstractions.Entities;
using System.Threading;

namespace IPLogsFilter.Abstractions.Repositories
{
    public interface IIPLogsFilterRepository
    {
        List<LogRecord> ReadLogs();
        void WriteFiltredLogs(List<FiltredLogs> filtredLogs);
        Task LoggingLogAndStateFromFileToDatabaseAsync(LogRecord log, string logFilePath, int currentLine, CancellationToken cancellationToken);
        Task LoggingLogsFromFileToDatabaseAsync(List<LogRecord> logRecords, CancellationToken cancellationToken);
        Task CompletingLogReadingAsync(string logFilePath, CancellationToken cancellationToken);
        Task<bool> IsProcessedLogFileAsync(string logFilePath, CancellationToken cancellationToken);
        Task<int> GetLastUnprocessedLineAsync(string logFilePath, CancellationToken cancellationToken);
    }
}
