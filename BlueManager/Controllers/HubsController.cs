using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlueManager.Data;
using BlueManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlueManager.Controllers
{
    [Authorize]
    public class HubsController : Controller
    {
        private readonly BlueManagerContext _context;
     
        public HubsController(BlueManagerContext context)
        {
            _context = context;
        }

        // GET: Hubs
        public async Task<IActionResult> Index()
        {
            var hubsList = await _context.Hubs.ToListAsync();
            return View(hubsList);
        }

        // GET: Hubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hub = await _context.Hubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hub == null)
            {
                return NotFound();
            }

            return View(hub);
        }

        // GET: Hubs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hubs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hub hub)
        {
            if (ModelState.IsValid)
            {
                hub = Trim(hub);
                if (IpExists(hub.IpAddress) == true)
                {
                    ModelState.AddModelError(string.Empty, "Koncentrator o takim adresie IP już istnieje");
                    return View(hub);
                }
                else
                {                    
                    _context.Add(hub);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(hub);
        }

        // GET: Hubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
       
            var hub = await _context.Hubs.FindAsync(id);
            if (hub == null)
            {
                return NotFound();
            }
            return View(hub);
        }

        // POST: Hubs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hub hub)
        {
            if (id != hub.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    hub = Trim(hub);
                    if (CanEdit(hub) == true)
                    {
                        ModelState.AddModelError(string.Empty, "Koncentrator o takim adresie IP już istnieje");
                        return View(hub);
                    }
                    else
                    {                      
                        _context.Update(hub);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HubExists(hub.Id))
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
            return View(hub);
        }

        // GET: Hubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hub = await _context.Hubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hub == null)
            {
                return NotFound();
            }

            return View(hub);
        }

        // POST: Hubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hub = await _context.Hubs.FindAsync(id);           
            _context.Hubs.Remove(hub);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HubExists(int id)
        {
            return _context.Hubs.Any(e => e.Id == id);
        }

        private bool IpExists(string ip)
        {
            return _context.Hubs.Any(e => e.IpAddress == ip);
        }

        private bool CanEdit(Hub hub)
        {   
            var _hub = _context.Hubs.Where(x => x.IpAddress == hub.IpAddress).SingleOrDefault();
            if (_hub != null)
            {
                if (_hub.Id == hub.Id && _hub.IpAddress == hub.IpAddress)
                {
                    _context.Entry(_hub).State = EntityState.Detached;
                    return false;
                }
                else if (_hub.Id != hub.Id && _hub.IpAddress == hub.IpAddress)
                {
                    _context.Entry(_hub).State = EntityState.Detached;
                    return true;
                }
            }
            return false;
        }

        private Hub Trim(Hub hub)
        {
            char[] charsToTrim = { '*', ' ', '\'', '\t' };
            if (hub.LocationName != null) hub.LocationName = hub.LocationName.Trim(charsToTrim);
            if (hub.IpAddress != null) hub.IpAddress = hub.IpAddress.Trim(charsToTrim);

            return hub;
        }
    }
}
