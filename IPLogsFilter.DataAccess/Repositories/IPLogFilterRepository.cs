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

        public async Task LoggingLogsFromFileToDatabaseAsync(List<LogRecord> logRecords, CancellationToken cancellationToken)
        {
            await _context.LogRecords.AddRangeAsync(logRecords, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public List<LogRecord> ReadLogs()
        {
            return _context.LogRecords.ToList();
        }

        public void WriteFiltredLogs(List<FiltredLogs> filtredLogs)
        {
            _context.FiltredLogs.AddRange(filtredLogs);
            _context.SaveChanges();
        }
    }
}
