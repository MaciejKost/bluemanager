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
        private static List<ToolAtHub> _cachedTools = new List<ToolAtHub>();

        public LocationsController(BlueManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortProperty, bool descending, string searchString, CancellationToken cancellationToken = new CancellationToken())
        {
            ViewBag.SortProperty = sortProperty;
            ViewBag.SortDescending = descending;
            List<ToolAtHub> sortedTools = null;
            List<ToolAtHub> newList = new List<ToolAtHub>();
            List<ToolAtHub> newList2 = new List<ToolAtHub>();
            ToolAtHub lastTool = new ToolAtHub();
            bool isOk;
            await using (_context)
            {
                try
                {
                    //   (sortedTools, isOk) = await GetSortedListOfTools(sortProperty, descending, searchString, cancellationToken);
                    ViewBag.Error = "";


                   
                     var toolNames = await _context.Tools.ToListAsync(cancellationToken);
                   //  var lastTool2 = await _context.ToolAtHubs.ToListAsync(cancellationToken);
                    //   var lastTool3 = await _context.ToolAtHubs.Include(x => x.Tool).Include(x => x.Hub).ToListAsync(cancellationToken);

                    foreach (var tool in toolNames)
                    {
                        lastTool = await _context.ToolAtHubs.OrderByDescending(t => t.Timestamp).Include(x => x.Tool).Include(x => x.Hub).Where(x => x.ToolId == tool.Id).FirstOrDefaultAsync(cancellationToken);
                        if (lastTool != null)
                        {
                            newList.Add(lastTool);
                        }
                    }
                    isOk = true;
                }
                catch (Exception ex)
                {
                    isOk = false;
                    Console.WriteLine(ex);
                }
            }
            // if (!isOk)
            //{
            //    ViewBag.Error = "Błąd połączenia z bazą danych";
            //}

            //return View(sortedTools);
            return View(newList);
        }

        private async Task<(List<ToolAtHub>, bool)> GetSortedListOfTools(string sortProperty, bool descending = false, string searchString = null, CancellationToken cancellationToken = new CancellationToken())
        {
            // will stop when cancellationToken is requested, or after 5 seconds
            var timeoutCts = new CancellationTokenSource();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            bool isOk = true;

            try
            {
                timeoutCts.CancelAfter(5000);
                // _cachedTools = await _context.Tools.ToListAsync(cts.Token);
                _cachedTools = await _context.ToolAtHubs.ToListAsync(cts.Token);
                var _toolNames = await _context.Tools.ToListAsync(cts.Token);



            }
            catch (Exception ex)
            {
                isOk = false;
                Console.WriteLine(ex);
            }

            IEnumerable<ToolAtHub> tools = _cachedTools;

            if (!String.IsNullOrEmpty(searchString))
            {
                tools = tools.Where(t => t.Tool.ToolName.Contains(searchString));
            }

            Func<ToolAtHub, IComparable> sortExpression = sortProperty switch
            {
                "obj_name" => t => t.Tool.ToolName,
                "name" => t => t.BleName,
                "location" => t => t.Hub.LocationName,
                "time" => t => t.Timestamp,
                _ => t => t.Tool.ToolName
            };

            tools = descending
                ? tools.OrderByDescending(sortExpression)
                : tools.OrderBy(sortExpression);
            //  return (tools.ToList(), isOk);
            return (_cachedTools.ToList(), isOk);

        }


    }
}