﻿using IPLogsFilter.Abstractions.Services;
using IPLogsFilterMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IPLogsFilterMVC.Controllers
{
    public class LogsController : Controller
    {
        private readonly ILogger<LogsController> _logger;
        private readonly ILogFilterService _logFilterService;

        public LogsController(ILogger<LogsController> logger, ILogFilterService logFilterService)
        {
            _logger = logger;
            _logFilterService = logFilterService;
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