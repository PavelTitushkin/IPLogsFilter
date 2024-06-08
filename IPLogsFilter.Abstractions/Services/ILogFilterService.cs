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
        void WriteLogsToDb(List<FiltredLogs?> filterLogs);
    }
}
