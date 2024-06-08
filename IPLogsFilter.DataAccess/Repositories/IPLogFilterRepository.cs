using IPLogsFilter.Abstractions.Entities;
using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Db;

namespace IPLogsFilter.DataAccess.Repositories
{
    public class IPLogFilterRepository : IIPLogsFilterRepository
    {
        private readonly IPLogsFilterContext _context;

        public IPLogFilterRepository(IPLogsFilterContext context)
        {
            _context = context;
        }

        public List<LogRecord> ReadLogs()
        {
            return _context.LogRecords.ToList();
        }

        public void WriteLogs(List<FiltredLogs> filtredLogs)
        {
            _context.FiltredLogs.AddRange(filtredLogs);
            _context.SaveChanges();
        }
    }
}
