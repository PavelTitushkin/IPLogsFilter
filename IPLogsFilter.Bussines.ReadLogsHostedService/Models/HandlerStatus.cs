namespace IPLogsFilter.Bussines.ReadLogsHostedService.Models
{
    public class HandlerStatus
    {
        public int ProcessedFilesCount { get; set; }
        public int TotalFilesToProcess { get; set; }
        public DateTime? LastRunStartTime { get; set; }
        public DateTime? LastRunEndTime { get; set; }
        public bool IsRunning { get; set; }
    }
}
