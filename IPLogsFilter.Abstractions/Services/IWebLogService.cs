using IPLogsFilter.Abstractions.Entities;

namespace IPLogsFilter.Abstractions.Services
{
    public interface IWebLogService
    {
        Task<IEnumerable<LogFile>>? GetLogFilesAsync(DateTime time, CancellationToken cancellationToken);
        Task<LogFile> GetLogAsync(string filename, CancellationToken cancellationToken);
    }
}
