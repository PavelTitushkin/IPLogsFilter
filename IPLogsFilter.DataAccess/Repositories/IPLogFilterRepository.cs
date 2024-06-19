using IPLogsFilter.Abstractions.DTOs;
using IPLogsFilter.Abstractions.Entities;
using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Db;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace IPLogsFilter.DataAccess.Repositories
{
    public class IPLogFilterRepository : IIPLogsFilterRepository
    {
        private readonly IPLogsFilterContext _context;

        public IPLogFilterRepository(IPLogsFilterContext context)
        {
            _context = context;
        }

        public async Task CompletingLogReadingAsync(string logFilePath, CancellationToken cancellationToken)
        {
            var stateEntity = await _context.StatusLoggingLogs.FirstOrDefaultAsync(log => log.FileName == logFilePath, cancellationToken);
            stateEntity.IsProcessed = true;
            _context.StatusLoggingLogs.Update(stateEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetLastUnprocessedLineAsync(string logFilePath, CancellationToken cancellationToken)
        {
            return await _context.StatusLoggingLogs
                .Where(log => log.FileName == logFilePath)
                .Select(l => l.LastLine)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<RestDTO<List<LogRecord>>> Get(int pg, int pageSize, string? sortColumn, string? sortOrder)
        {
            var query = _context.LogRecords.AsQueryable();
            var logsCount = await query.CountAsync();
            query = query
                .AsNoTracking()
                .OrderBy($"{sortColumn} {sortOrder}")
                .Skip(pg * pageSize)
                .Take(pageSize);

            return new RestDTO<List<LogRecord>>()
            {
                Data = await query.ToListAsync(),
                PageIndex = pg,
                PageSize = pageSize,
                RecordCount = logsCount
            };
        }

        public async Task<bool> IsProcessedLogFileAsync(string logFilePath, CancellationToken cancellationToken)
        {
            return await _context.StatusLoggingLogs.Where(s => s.IsProcessed).AnyAsync(cancellationToken);
        }

        public async Task LoggingLogAndStateFromFileToDatabaseAsync(LogRecord log, string logFilePath, int currentLine, CancellationToken cancellationToken)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var isExistStateEntity = await _context.StatusLoggingLogs.FirstOrDefaultAsync(log => log.FileName == logFilePath, cancellationToken);
                        if (isExistStateEntity != null)
                        {
                            isExistStateEntity.LastLine = currentLine;
                            isExistStateEntity.IsProcessed = false;
                        }
                        else
                        {
                            await _context.StatusLoggingLogs.AddAsync(new StatusLoggingLogs
                            {
                                FileName = logFilePath,
                                IsProcessed = false,
                                LastLine = currentLine,
                            },
                            cancellationToken);
                        }

                        await _context.LogRecords.AddAsync(log, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                }
            });
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
