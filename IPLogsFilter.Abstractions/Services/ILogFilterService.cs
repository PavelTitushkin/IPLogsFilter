using IPLogsFilter.Abstractions.DTOs;
using IPLogsFilter.Abstractions.Entities;

namespace IPLogsFilter.Abstractions.Services
{
    public interface ILogFilterService
    {
        List<LogRecord> GetLogRecords();
        List<LogRecord> FilterLogs(
            List<LogRecord> logs,
            string? addressStart, 
            string? addressMask, 
            DateTime timeStart, 
            DateTime timeEnd);
        List<FiltredLogs> CountingIPVisits(List<LogRecord> logs);
        void WriteFiltredLogsToDb(List<FiltredLogs?> filterLogs);
        Task ReadLogsFromFileAsync(string logFilePath, CancellationToken cancellationToken);
        Task<RestDTO<List<LogRecord>>> Get(int pg, int pageSize, string? sortColumn, string? sortOrder);
    }
}
