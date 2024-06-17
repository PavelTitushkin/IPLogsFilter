using Microsoft.AspNetCore.Mvc;
using WebLogsProvider.Contracts;
using WebLogsProvider.Models;

namespace WebLogsProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;
        private readonly IWebLogProccessor _webLogProccessor;

        public LogsController(ILogger<LogsController> logger, IWebLogProccessor webLogProccessor)
        {
            _logger = logger;
            _webLogProccessor = webLogProccessor;
        }

        [HttpGet("files")]
        public async Task<IEnumerable<LogFile?>> Get([FromQuery] DateTime? time, CancellationToken cancellationToken)
        {
            return await _webLogProccessor.GetLogsByTimeAsync(time, cancellationToken);

            //return logFiles != null ? Ok(logFiles) : NotFound();
        }

        [HttpGet("files/{filename}")]
        public IActionResult Get(string filename)
        {
            var logFile = _webLogProccessor.GetLog(filename);

            return logFile != null ? Ok(logFile) : NotFound();
        }
    }
}
