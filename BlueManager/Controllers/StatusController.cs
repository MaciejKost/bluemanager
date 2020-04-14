﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlueManager.Services.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;


namespace BlueManager.Controllers
{
    public class StatusController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StatusController> _logger;
        public static int errorNr;
        public static int notActiveNr;

        public List<string> Messages { get; set; }
        public StatusController(IHttpClientFactory httpClientFactory, ILogger<StatusController> logger)
        {        
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Status()
        {
            var statusList = new List<HubData>();
            var url = "https://localhost:5001/health";
            try
            {
                using var http = _httpClientFactory.CreateClient();
                http.Timeout = TimeSpan.FromMilliseconds(5000);               
                var reportString = await http.GetStringAsync(url);
                var report = JsonConvert.DeserializeObject<HealthSystemReport>(reportString);
                notActiveNr = 0;
                errorNr = 0;
                foreach (var item in report.entries.Hubs.Data.Values)
                {
                     statusList.Add(item);
                    if (item.HealthStatus == "Degraded")
                    {
                        notActiveNr++;
                    }
                    else if (item.HealthStatus == "Unhealthy")
                    {
                        errorNr++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error downloading report for HubId: from URL: {url}");
            }

            return View(statusList);
        }

        public ActionResult GetStatus()
        {
     

            return Json(new { notActive = notActiveNr, error = errorNr });

        }
    }
}


