using IPLogsFilter.Abstractions.Entities;

namespace IPLogsFilter.Abstractions.Repositories
{
    public interface IIPLogsFilterRepository
    {
        List<LogRecord> ReadLogs();
        void WriteFiltredLogs(List<FiltredLogs> filtredLogs);
        Task LoggingLogsFromFileToDatabaseAsync(List<LogRecord> logRecords, CancellationToken cancellationToken);
    }
}
