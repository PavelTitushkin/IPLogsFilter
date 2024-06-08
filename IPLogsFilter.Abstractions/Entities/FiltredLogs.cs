namespace IPLogsFilter.Abstractions.Entities
{
    public class FiltredLogs
    {
        public int Id { get; set; }
        public string LogRecord { get; set; }
        public int CountLogRecords { get; set; }
    }
}
