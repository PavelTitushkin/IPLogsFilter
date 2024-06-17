using IPLogsFilter.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace IPLogsFilterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IWebLogService _webLogService;

        public LogsController(IWebLogService webLogService)
        {
            _webLogService = webLogService;
        }

        [HttpGet("time")]
        public async Task<IActionResult> GetLogFilesFromApi(DateTime time, CancellationToken cancellationToken)
        {
            var files = await _webLogService.GetLogFilesAsync(time, cancellationToken);

            return files != null ? Ok(files) : NotFound();
        }

        [HttpGet("filename")]
        public async Task<IActionResult> GetLogFromApi(string filename, CancellationToken cancellationToken)
        {
            var files = await _webLogService.GetLogAsync(filename, cancellationToken);

            return files != null ? Ok(files) : NotFound();
        }
    }
}
