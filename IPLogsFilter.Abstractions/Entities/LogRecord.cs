using System.Net;

namespace IPLogsFilter.Abstractions.Entities
{
    public class LogRecord
    {
        public int Id { get; set; }
        public DateTime RequestTime { get; set; }
        public string ApplicationName { get; set; }
        public string Stage { get; set; }
        public IPAddress ClientIpAddress { get; set; }
        public string ClientName { get; set; }
        public string ClientVersion { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string StatusCode { get; set; }
        public string StatusMesage { get; set; }
        public string ContentType { get; set; }
        public string ContentLength { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public Int32 MemroyUsage { get; set; }
    }
}
