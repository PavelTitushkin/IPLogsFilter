using System.Net;

namespace IPLogFilter.Models
{
    public class LogRecord
    {
        public IPAddress IPAddress { get; set; }
        public DateTime TimeVisit { get; set; }
    }
}
