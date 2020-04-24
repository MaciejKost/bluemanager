using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlueManager.Services.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using StackExchange.Profiling;
using Microsoft.Extensions.Caching.Memory;

namespace BlueManager.Controllers
{
    public class StatusController : Controller
    {
        private readonly ILogger<StatusController> _logger;
        private readonly IMemoryCache _cache;
        public static int errorNr;
        public static int notActiveNr;


        public StatusController(ILogger<StatusController> logger, IMemoryCache memoryCache)

        {
            _logger = logger;
            _cache = memoryCache;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Status()
        {
            var statusList = new List<CheckReport>();

            try
            {
                var report = _cache.Get<Dictionary<string, CheckReport>>(CacheKeys.Entry);
                notActiveNr = 0;
                errorNr = 0;
                foreach (var item in report.Values)
                {
                    statusList.Add(item);
                    if (!item.IsActive)
                    {
                        notActiveNr++;
                    }
                    else if (!item.Status)
                    {
                        errorNr++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cache reading");
            }

            return View(statusList);
        }

        public JsonResult Status2()
        {
            var statusList = new List<CheckReport>();
            StatusRaport sts;

            try
            {
                var report = _cache.Get<Dictionary<string, CheckReport>>(CacheKeys.Entry);
                notActiveNr = 0;
                errorNr = 0;
                foreach (var item in report.Values)
                {
                    statusList.Add(item);
                    if (!item.IsActive)
                    {
                        notActiveNr++;
                    }
                    else if (!item.Status)
                    {
                        errorNr++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cache reading");
            }

            sts = new StatusRaport() { Error = errorNr, NotActive = notActiveNr, StatusList = statusList };

            return Json(sts);
        }

    }

    public class StatusRaport
    {
        public int NotActive { get; set; }
        public int Error { get; set; }
        public List<CheckReport> StatusList { get; set; }
    }
}


