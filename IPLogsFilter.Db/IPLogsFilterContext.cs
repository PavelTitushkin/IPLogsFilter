using IPLogsFilter.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPLogsFilter.Db
{
    public class IPLogsFilterContext : DbContext
    {
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<FiltredLogs> FiltredLogs { get; set; }
        public IPLogsFilterContext(DbContextOptions<IPLogsFilterContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
