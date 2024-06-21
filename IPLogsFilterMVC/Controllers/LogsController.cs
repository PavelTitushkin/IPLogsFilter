using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Contracts;
using IPLogsFilter.Bussines.ReadLogsHostedService.Services;
using IPLogsFilterMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IPLogsFilterMVC.Controllers
{
    [Authorize(Policy = "WebMasterOrWebAdmin")]
    public class LogsController : Controller
    {
        private readonly ILogger<LogsController> _logger;
        private readonly ILogFilterService _logFilterService;
        private readonly ILogBackgroundReader _backgroundReader;

        public LogsController(ILogger<LogsController> logger, ILogFilterService logFilterService, ILogBackgroundReader backgroundReader)
        {
            _logger = logger;
            _logFilterService = logFilterService;
            _backgroundReader = backgroundReader;
        }

        public async Task<IActionResult> Index(
            int pg = 1, 
            int pageSize = 5, 
            string? sortColumn = "RequestTime", 
            string? sortOrder = "ASC")
        {
            var logs = await _logFilterService.Get(pg, pageSize, sortColumn, sortOrder);

            if (pg < 1)
            {
                pg = 1;
            }
            if (logs != null)
            {
                var pager = new Pager((int)logs.RecordCount, pg, pageSize);

                ViewBag.Pager = pager;

                return View(logs.Data);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult GetStatusHandler()
        {
            var status = _backgroundReader.GetStatusHandler();

            return View(status);
        }

        [Authorize(Policy = "ProcessingStop")]
        public IActionResult StopHandler()
        {
            _backgroundReader.StopHandler(false);

            return View();
        }

        [Authorize(Policy = "ProcessingStart")]
        public IActionResult StartHandler(CancellationToken cancellationToken)
        {
            _backgroundReader.StartHandler(cancellationToken);

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
