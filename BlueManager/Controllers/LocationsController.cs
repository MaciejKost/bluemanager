using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
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
        private static List<Tool> _cachedTools = new List<Tool>();

        public LocationsController(BlueManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortProperty, bool descending, string searchString, CancellationToken cancellationToken = new CancellationToken())
        {
            ViewBag.SortProperty = sortProperty;
            ViewBag.SortDescending = descending;
            List<Tool> sortedTools = null;
            bool isOk;

            try
            {
                (sortedTools, isOk) = await GetSortedListOfTools(sortProperty, descending, searchString, cancellationToken);
                ViewBag.Error = "";
            }
            catch (Exception ex)
            {
                isOk = false;
                Console.WriteLine(ex);
            }

            if (!isOk)
            {
                ViewBag.Error = "Błąd połączenia z bazą danych";
            }

            return View(sortedTools);
        }

        private async Task<(List<Tool>, bool)> GetSortedListOfTools(string sortProperty, bool descending = false, string searchString = null, CancellationToken cancellationToken = new CancellationToken())
        {
            // will stop when cancellationToken is requested, or after 5 seconds
            var timeoutCts = new CancellationTokenSource();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            bool isOk = true;

            try
            {
                timeoutCts.CancelAfter(5000);
                _cachedTools = await _context.Tools.ToListAsync(cts.Token);
            }
            catch (Exception ex)
            {
                isOk = false;
                Console.WriteLine(ex);
            }

            IEnumerable<Tool> tools = _cachedTools;

            if (!String.IsNullOrEmpty(searchString))
            {
                tools = tools.Where(t => t.ToolName.Contains(searchString));
            }

            Func<Tool, IComparable> sortExpression = sortProperty switch
            {
                "obj_name" => t => t.ToolName,
                //"name"     => t => t.Name,
                //"location" => t => t.Location,
                //"time"     => t => t.Time,
                _          => t => t.ToolName
            };

            tools = descending
                ? tools.OrderByDescending(sortExpression)
                : tools.OrderBy(sortExpression);

            return (tools.ToList(), isOk);

        }


    }
}