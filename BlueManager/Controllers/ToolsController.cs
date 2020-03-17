﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlueManager.Data;
using BlueManagerPlatform.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlueManager.Controllers
{
    [Authorize]
    public class ToolsController : Controller
    {
        private readonly BlueManagerContext _context;

        public ToolsController(BlueManagerContext context)
        {
            _context = context;
        }

        // GET: Tools
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "obj_name_desc" : "";
           // ViewBag.NameSortParm = sortOrder == "obj_name" ? "obj_name_desc" : "obj_name";
            ViewBag.ObjNameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.LocationSortParm = sortOrder == "location" ? "location_desc" : "location";
            ViewBag.TimeSortParm = sortOrder == "time" ? "time_desc" : "time";
            var tools = from t in _context.Tools
                        select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tools = tools.Where(t => t.ObjName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    tools = tools.OrderByDescending(t => t.Name);
                    break;
                case "obj_name_desc":
                    tools = tools.OrderByDescending(t => t.ObjName);
                    break;
                case "obj_name":
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
                    tools = tools.OrderBy(t => t.ObjName);
                    break;
            }

            return View(await tools.ToListAsync());
        }

        // GET: Tools/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tool = await _context.Tools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            return View(tool);
        }

        // GET: Tools/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ObjName,MacAddress,Location,Time")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                tool = Trim(tool);
                if (MacExists(tool.MacAddress) == true)
                {
                    ViewBag.Error = "Narzędzie o takim adresie MAC już istnieje";
                    return View(tool);
                }
                else
                {
                    ViewBag.Error = "";                  
                    _context.Add(tool);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(tool);
        }

        // GET: Tools/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool = await _context.Tools.FindAsync(id);
            if (tool == null)
            {
                return NotFound();
            }
            return View(tool);
        }

        // POST: Tools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ObjName,MacAddress,Location,Time")] Tool tool)
        {
            if (id != tool.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tool = Trim(tool);
                    if (CanEdit(tool))
                    {
                        ViewBag.Error = "Narzędzie o takim adresie MAC już istnieje";
                        return View(tool);                   
                    }
                    else
                    {
                        ViewBag.Error = "";
                        _context.Update(tool);
                        await _context.SaveChangesAsync();
                    }
 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToolExists(tool.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tool);
        }

        // GET: Tools/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool = await _context.Tools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            return View(tool);
        }

        // POST: Tools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tool = await _context.Tools.FindAsync(id);
            _context.Tools.Remove(tool);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ToolExists(int id)
        {
            return _context.Tools.Any(e => e.Id == id);
        }

        private bool MacExists(string mac)
        {
            return _context.Tools.Any(e => e.MacAddress == mac);
        }

        private bool CanEdit(Tool tool)
        {
            var _tool = _context.Tools.Where(x => x.MacAddress == tool.MacAddress).SingleOrDefault();
            if (_tool != null)
            {
                if (_tool.Id == tool.Id && _tool.MacAddress == tool.MacAddress)
                {
                    _context.Entry(_tool).State = EntityState.Detached;
                    return false;
                }
                else if (_tool.Id != tool.Id && _tool.MacAddress == tool.MacAddress)
                {
                    _context.Entry(_tool).State = EntityState.Detached;
                    return true;
                }
            }  
            return false;
        }

        private Tool Trim(Tool tool)
        {
            char[] charsToTrim = { '*', ' ', '\'', '\t' };
            if (tool.Location != null) tool.Location = tool.Location.Trim(charsToTrim);
            if (tool.MacAddress != null) tool.MacAddress = tool.MacAddress.Trim(charsToTrim);
            if (tool.Name != null) tool.Name = tool.Name.Trim(charsToTrim);
            if (tool.ObjName != null) tool.ObjName = tool.ObjName.Trim(charsToTrim);

            return tool;
        }
    }
}