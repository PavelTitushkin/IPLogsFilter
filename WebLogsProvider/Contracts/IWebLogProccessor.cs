using WebLogsProvider.Models;

namespace WebLogsProvider.Contracts
{
    public interface IWebLogProccessor
    {
        Task<IEnumerable<LogFile?>> GetLogsByTimeAsync(DateTime? time, CancellationToken cancellationToken);
        LogFile? GetLog(string filename);
    }
}
