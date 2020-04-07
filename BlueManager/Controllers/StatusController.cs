using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Services.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;


namespace BlueManager.Controllers
{
    public class StatusController : Controller
    {
        private readonly BlueManagerContext _context;
        private readonly HealthCheckService _health;
        private readonly ILogger<StatusController> _logger;

        public List<string> Messages { get; set; }
        public StatusController(BlueManagerContext context, HealthCheckService health, ILogger<StatusController> logger)
        {
            _context = context;
            _health = health;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Status()
        {        
            //var Messages = new List<string>();
            var statusList = new List<CheckReport>();
            var report = await _health.CheckHealthAsync();

            foreach (var item in report.Entries["Hubs"].Data)
            {
                //var ipAddress = item.Key;
                //var status = (CheckReport)item.Value;
                //Messages.Add($"{status.LocationName}.({status.IpAddress}) : {status.Status.ToString()}.");
                statusList.Add((CheckReport)item.Value);
            }
           
            return View(statusList);
        }
    }
}


