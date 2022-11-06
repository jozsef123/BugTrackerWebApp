using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerWebApp.Controllers
{
    public class TicketsController :  UsersController
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context) : base(context)
    {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "closed" : "Status";
            ViewData["DateCreatedSortParm"] = sortOrder == "DateCreated" ? "dateCreated_desc" : "DateCreated";
            ViewData["DateUpdatedSortParm"] = sortOrder == "DateUpdated" ? "dateUpdated_desc" : "DateUpdated";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            // TODO: Should Admin see all tickets?
            var tickets = GetCurrentUserTickets();

            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets
                    .Where(t => 
                        t.Name.Contains(searchString) ||
                        t.Project.Name.Contains(searchString) ||
                        t.Description.Contains(searchString) ||
                        t.Submitter.UserName.Contains(searchString) ||
                        t.AssignedDeveloper.UserName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Status":
                    tickets = tickets.OrderBy(s => s.Status);
                    break;
                case "closed":
                    tickets = tickets.OrderByDescending(s => s.Status);
                    break;
                case "DateCreated":
                    tickets = tickets.OrderBy(s => s.CreatedWhen);
                    break;
                case "dateCreated_desc":
                    tickets = tickets.OrderByDescending(s=>s.CreatedWhen);
                    break;
                case "DateUpdated":
                    tickets = tickets.OrderBy(s => s.UpdatedWhen);
                    break;
                case "dateUpdated_desc":
                    tickets = tickets.OrderByDescending(s => s.UpdatedWhen);
                    break;
                default:
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Ticket>.CreateAsync(tickets.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Tickets/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ticket = GetTicketById(id).First();
            if (ticket == null) return NotFound();

            if (GetCurrentUserRoles().Contains("Admin") || IsUserTicket(ticket)) 
            {
                return View(ticket);
            }
            return NoContent();
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {

            var projects = GetCurrentUserProjects();
            var users = GetUsersInCurrentUserProjects();
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.Projects = new SelectList(projects, "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,Name,Description,SubmitterId,AssignedDeveloperId,Priority,Type,Status,CreatedWhen,UpdatedWhen")] Ticket ticket)
        {
            // TODO tickets should have unique names?
            if (ticket.AssignedDeveloperId != null && !IsUserInProject(ticket.AssignedDeveloperId, ticket.ProjectId))
            {
                var projects = GetCurrentUserProjects();
                var users = GetUsersInCurrentUserProjects();
                ViewBag.Users = new SelectList(users, "Id", "UserName");
                ViewBag.Projects = new SelectList(projects, "Id", "Name");
                ViewBag.ErrorMessage = "Assigned Developer does not exist in the selected project";
            }
            else if (ModelState.IsValid)
            {
                ticket.Submitter = GetCurrentUser();
                ticket.CreatedWhen = DateTime.Now;

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                var ticketHistory = new TicketHistory()
                {
                    TicketId = ticket.Id,
                    PreviousAssignedDeveloperId = null,
                    NewAssignedDeveloperId = ticket.AssignedDeveloperId,
                    UpdatedWhen = DateTime.Now,
                    Updater = GetCurrentUser()
                };
                _context.TicketHistory.Add(ticketHistory);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ticket = GetTicketById(id).AsNoTracking().First();
            if (ticket == null) return NotFound();

            if (GetCurrentUserRoles().Contains("Admin") || IsUserTicket(ticket))
            {
                ViewBag.Users = new SelectList(GetUsersInProject(ticket.Project),"Id", "UserName");
                return View(ticket);
            }
            else
            {
                return NoContent();
            }
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,Name,Description,SubmitterId,AssignedDeveloperId,Priority,Type,Status,CreatedWhen,UpdatedWhen")] Ticket ticket)
        {

            if (id != ticket.Id)
            {
                return NotFound();
            }

            // Tutorial: Update related data - ASP.NET MVC with EF Core
            // Source used: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/update-related-data?view=aspnetcore-6.0            
            string PreviousDeveloperId = GetTicketById(id).AsNoTracking().First().AssignedDeveloperId;
            string NewDeveloperId = ticket.AssignedDeveloperId;
            
            if (ModelState.IsValid)
            {
                try
                {
                    ticket.UpdatedWhen = DateTime.Now;
                    _context.Update(ticket);
                    var ticketHistory = new TicketHistory()
                    {
                        TicketId = ticket.Id,
                        PreviousAssignedDeveloperId = PreviousDeveloperId,
                        NewAssignedDeveloperId = NewDeveloperId,
                        UpdatedWhen = DateTime.Now,
                        Updater = GetCurrentUser()
                    };
                    _context.TicketHistory.Add(ticketHistory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    // Log the error
                    ModelState.AddModelError("", "Unable to save changes. " +
                                    "Try again, and if the problem persists, " +
                                    "see your system administrator.");
                }
            }
            return View(ticket);

        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = GetTicketById(id).First();
            if (ticket == null)
            {
                return NotFound();
            }
            if (GetCurrentUserRoles().Contains("Admin") || IsUserTicket(ticket))
            {
                return View(ticket);
            }
            return NoContent();
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
