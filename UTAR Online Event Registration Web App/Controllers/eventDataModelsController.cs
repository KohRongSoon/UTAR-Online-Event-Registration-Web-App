using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UTAR_Online_Event_Registration_Web_App.Data;
using UTAR_Online_Event_Registration_Web_App.Models;

namespace UTAR_Online_Event_Registration_Web_App.Controllers
{
    public class eventDataModelsController : Controller
    {
        private readonly UTAR_Online_Event_Registration_Web_AppContext _context;

        public eventDataModelsController(UTAR_Online_Event_Registration_Web_AppContext context)
        {
            _context = context;
        }

        // GET: eventDataModels
        public IActionResult Index()
        {
            return View(_context.eventDataModel.ToList());
        }

        // GET: eventDataModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventDataModel = await _context.eventDataModel
                .FirstOrDefaultAsync(m => m.eventID == id);
            if (eventDataModel == null)
            {
                return NotFound();
            }

            return View(eventDataModel);
        }

        // GET: eventDataModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: eventDataModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("eventID,userId,eventName,eventDescription,eventDate,eventPrice,eventTicQty")] EventDataModel eventDataModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventDataModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventDataModel);
        }

        // GET: eventDataModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventDataModel = await _context.eventDataModel.FindAsync(id);
            if (eventDataModel == null)
            {
                return NotFound();
            }
            return View(eventDataModel);
        }

        // POST: eventDataModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("eventID,userId,eventName,eventDescription,eventDate,eventPrice,eventTicQty")] EventDataModel eventDataModel)
        {
            if (id != eventDataModel.eventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventDataModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!eventDataModelExists(eventDataModel.eventID))
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
            return View(eventDataModel);
        }

        // GET: eventDataModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventDataModel = await _context.eventDataModel
                .FirstOrDefaultAsync(m => m.eventID == id);
            if (eventDataModel == null)
            {
                return NotFound();
            }

            return View(eventDataModel);
        }

        // POST: eventDataModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventDataModel = await _context.eventDataModel.FindAsync(id);
            _context.eventDataModel.Remove(eventDataModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool eventDataModelExists(int id)
        {
            return _context.eventDataModel.Any(e => e.eventID == id);
        }
    }
}
