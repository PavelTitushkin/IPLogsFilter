using IPLogsFilter.Abstractions.Entities;
using IPLogsFilter.Abstractions.Repositories;

namespace IPLogsFilter.DataAccess.Repositories
{
    public class IPLogRepository : IIPLogsFilterRepository
    {
        public List<LogRecord> ReadLogs()
        {
            throw new NotImplementedException();
        }

        public void WriteLogs()
        {
            throw new NotImplementedException();
        }
    }
}
