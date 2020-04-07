using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BlueManager.Controllers
{
    public class LocationsController : Controller
    {
        private readonly BlueManagerContext _context;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(BlueManagerContext context, ILogger<LocationsController> logger)
        {
            _context = context;
            _logger = logger;
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
                    _logger.LogCritical(ex, "There was a problem with database at {Time}", DateTime.Now);
                    throw;
                }
            }
            return View(toolLastLocations);
        }

        public async Task<IActionResult> IndexShort(CancellationToken cancellationToken = new CancellationToken())
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
                    _logger.LogCritical(ex, "There was a problem with database at {Time}", DateTime.Now);
                    throw;
                }
            }
            return View(toolLastLocations);
        }

        public async Task<IActionResult> SearchTool(string searchString, CancellationToken cancellationToken = new CancellationToken())
        {
            ToolLastLocation toolLastLocation;
            await using (_context)
            {
                try
                {

                        toolLastLocation = await _context.ToolLastLocations
                          .Include(x => x.Tool)
                          .Include(x => x.Hub)
                          .Where(x => x.Tool.ToolName.Contains(searchString))
                          .FirstOrDefaultAsync(cancellationToken);
                
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "There was a problem with database at {Time}", DateTime.Now);
                    throw;
                }
            }
           
            return View(toolLastLocation);
        }
    }
}