using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BugTrackerWebApp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
  

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Ticket
                .Include(t => t.Project)
                .AsNoTracking();
            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            // get the name of project and send to viewbag

            var listData = (from ticket in _context.Ticket
                               join project in _context.Project
                               on ticket.ProjectId equals project.Id
                               where ticket.Id == id
                               select new
                               {
                                   projectName = project.Name,
                                   ticketName = ticket.Name,
                                   ticketId = ticket.Id
                               }
                               ).ToList();


            ViewBag.projectName = listData[0].projectName;
            //ViewBag.ticketName = listData[0].ticketName;
            //ViewBag.ticketId = listData[0].ticketId;
            TempData["ticketName"] = listData[0].ticketName;
            TempData["ticketId"] = listData[0].ticketId;

            var query = await _context.Ticket
               .Include(t => t.Comments)
               .AsNoTracking()
               .FirstOrDefaultAsync(m => m.Id == id);


            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, ProjectId, Name,Description,SubmitterUserName,AssignedDeveloperUserName,Priority,Type,Status,Date_Created,Date_Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.SubmitterUserName = User.Identity.Name;
                ticket.Date_Created = DateTime.Now;

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewBag.Users = new SelectList(_context.Users, "UserName");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,Name,Description,SubmitterUserName,AssignedDeveloperUserName,Priority,Type,Status,Date_Created,Date_Updated")] Ticket ticket)
        {

            if (id != ticket.Id)
            {
                return NotFound();
            }

            // Tutorial: Update related data - ASP.NET MVC with EF Core
            // Source used: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/update-related-data?view=aspnetcore-6.0
            var ticketToUpdate = await _context.Ticket.FirstOrDefaultAsync
                (t => t.Id == id);
            ticketToUpdate.Date_Updated = DateTime.Now;

            if (await TryUpdateModelAsync<Ticket>(ticketToUpdate, "",
                t=>t.ProjectId, 
                t=>t.Name, 
                t=>t.Description,
                t=>t.SubmitterUserName,
                t=>t.AssignedDeveloperUserName,
                t=>t.Priority,
                t=>t.Type,
                t=>t.Status,
                t=>t.Date_Created,
                t=>t.Date_Updated))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    // Log the error
                    ModelState.AddModelError("", "Unable to save changes. " +
                                    "Try again, and if the problem persists, " +
                                    "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticketToUpdate);

        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
