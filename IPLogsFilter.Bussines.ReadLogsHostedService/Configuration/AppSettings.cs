namespace IPLogsFilter.Bussines.ReadLogsHostedService.Configuration
{
    public class AppSettings
    {
        public string PathToFolderWithLogs { get; set; }
        public int TimeToCheckForNewRawFiles { get; set; }
        public int FilesToProcessPerIteration { get; set; }
    }
}
