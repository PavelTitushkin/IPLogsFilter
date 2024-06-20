using IPLogsFilter.Bussines.ReadLogsHostedService.Models;

namespace IPLogsFilter.Bussines.ReadLogsHostedService.Contracts
{
    public interface ILogBackgroundReader
    {
        HandlerStatus GetStatusHandler();
        void StartHandler(CancellationToken cancellationToken);
        void StopHandler(bool processRemaining);
    }
}
