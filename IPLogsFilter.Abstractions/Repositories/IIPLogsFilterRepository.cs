using IPLogsFilter.Abstractions.Entities;

namespace IPLogsFilter.Abstractions.Repositories
{
    public interface IIPLogsFilterRepository
    {
        List<LogRecord> ReadLogs();
        void WriteLogs();
    }
}
