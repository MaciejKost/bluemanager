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

        public LocationsController(BlueManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = new CancellationToken())
        {
       
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