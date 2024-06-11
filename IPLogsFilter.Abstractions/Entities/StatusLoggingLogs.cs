namespace IPLogsFilter.Abstractions.Entities
{
    public class StatusLoggingLogs
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool IsProcessed { get; set; }
        public int LastLine { get; set; }
    }
}
