namespace IPLogsFilter.Bussines.ReadLogsHostedService.Configuration
{
    public class AppSettingsMinio
    {
        public string BucketName { get; set; }
        public string PathFolderToSaveFile { get; set; }
        public int TimeToCheckForNewRawFiles { get; set; }
        public int FilesToProcessPerIteration { get; set; }
    }
}
