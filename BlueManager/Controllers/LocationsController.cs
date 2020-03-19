using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManagerPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace BlueManager.Controllers
{

    public class LocationsController : Controller
    {
        private readonly BlueManagerContext _context;
        private List<Tool> tools = new List<Tool>();
        public LocationsController(BlueManagerContext context)
        {
       
            try
            {
                _context = context;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, CancellationToken cancellationToken = new CancellationToken())
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "time_desc" : "";
            ViewBag.NameSortParm = sortOrder == "obj_name" ? "obj_name_desc" : "obj_name";
            ViewBag.ObjNameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.LocationSortParm = sortOrder == "location" ? "location_desc" : "location";
            //  ViewBag.TimeSortParm = sortOrder == "time" ? "time_desc" : "time";

            try
            {
                var tools = from t in _context.Tools
                            select t;

                if (!String.IsNullOrEmpty(searchString))
                {
                    tools = tools.Where(t => t.ObjName.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "obj_name":
                        tools = tools.OrderBy(t => t.Name);
                        break;
                    case "obj_name_desc":
                        tools = tools.OrderByDescending(t => t.Name);
                        break;
                    case "name_desc":
                        tools = tools.OrderByDescending(t => t.ObjName);
                        break;
                    case "name":
                        tools = tools.OrderBy(t => t.ObjName);
                        break;
                    case "location_desc":
                        tools = tools.OrderByDescending(t => t.Location);
                        break;
                    case "location":
                        tools = tools.OrderBy(t => t.Location);
                        break;
                    case "time_desc":
                        tools = tools.OrderByDescending(t => t.Time);
                        break;
                    case "time":
                        tools = tools.OrderBy(t => t.Time);
                        break;
                    default:
                        tools = tools.OrderByDescending(t => t.Time);
                        break;
                }
                ViewBag.Error = "";
                return View(await tools.ToListAsync());
            }
            catch (Exception)
            {
                ViewBag.Error = "Błąd połączenia z bazą danych";
               // var tools2 = new List<Tool>();
                return View(tools);
             //   return View(await tools.ToListAsync());
             
                throw;
            }

            
        }




    }
}