using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Models;
using BlueManagerPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace BlueManager.Controllers
{
    public class LocationsController : Controller
    {
        private readonly BlueManagerContext _context;
        private readonly HealthCheckService _health;

        public LocationsController(BlueManagerContext context, HealthCheckService health)
        {
            _context = context;
            _health = health;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = new CancellationToken())
        {

            var report = await _health.CheckHealthAsync();
            foreach (var item in report.Entries["Hubs"].Data)
            {
                var test = item.Key;
                var test2 = (bool)item.Value;
            }
            
            List<ToolLastLocation> toolLastLocations;
            await using (_context)
            {
                try
                {
                    toolLastLocations = await _context.ToolLastLocations
                        .Include(x => x.Tool)
                        .Include(x => x.Hub)
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return View(toolLastLocations);
        }

    }
}