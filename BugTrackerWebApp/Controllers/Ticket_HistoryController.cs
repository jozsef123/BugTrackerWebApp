using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;

namespace BugTrackerWebApp.Controllers
{
    public class Ticket_HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Ticket_HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ticket_History
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ticket_History.ToListAsync());
        }

        // GET: Ticket_History/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket_History = await _context.Ticket_History
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket_History == null)
            {
                return NotFound();
            }

            return View(ticket_History);
        }

        // GET: Ticket_History/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ticket_History/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TicketId,OldValueUserName,NewValueUserName,Date_Changed,TicketUpdaterUserName")] Ticket_History ticket_History)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket_History);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket_History);
        }

        // GET: Ticket_History/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket_History = await _context.Ticket_History.FindAsync(id);
            if (ticket_History == null)
            {
                return NotFound();
            }
            return View(ticket_History);
        }

        // POST: Ticket_History/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,OldValueUserName,NewValueUserName,Date_Changed,TicketUpdaterUserName")] Ticket_History ticket_History)
        {
            if (id != ticket_History.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket_History);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Ticket_HistoryExists(ticket_History.Id))
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
            return View(ticket_History);
        }

        // GET: Ticket_History/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket_History = await _context.Ticket_History
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket_History == null)
            {
                return NotFound();
            }

            return View(ticket_History);
        }

        // POST: Ticket_History/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket_History = await _context.Ticket_History.FindAsync(id);
            _context.Ticket_History.Remove(ticket_History);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Ticket_HistoryExists(int id)
        {
            return _context.Ticket_History.Any(e => e.Id == id);
        }
    }
}
