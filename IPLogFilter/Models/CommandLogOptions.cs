using System.Net;

namespace IPLogFilter.Models
{
    public class CommandLogOptions
    {
        public string FileLog { get; set; }
        public string FileOutput { get; set; }
        public string? AddressStart { get; set; }
        public string? AddressMask { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public List<LogRecord>? LogRecords { get; set; }
        public Dictionary<IPAddress, int> CountVisits { get; set; } = new Dictionary<IPAddress, int>();
    }
}
