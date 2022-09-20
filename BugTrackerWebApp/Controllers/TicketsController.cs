using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerWebApp.Controllers
{
    public class TicketsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;
  

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: Tickets
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            Console.WriteLine(sortOrder);
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

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var projectIds = from u in _context.User_Project
                               where u.UserId == userId
                               select u.ProjectId;

            var tickets = from t in _context.Ticket
                          where projectIds.Contains(t.ProjectId)
                          select t;
            tickets = tickets.Include(t => t.Project).AsNoTracking();


            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.Name.Contains(searchString)
                                     || t.Project.Name.Contains(searchString)
                                     || t.Description.Contains(searchString)
                                     || t.SubmitterUserName.Contains(searchString)
                                     || t.AssignedDeveloperUserName.Contains(searchString));
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
                    tickets = tickets.OrderBy(s => s.Date_Created);
                    break;
                case "dateCreated_desc":
                    tickets = tickets.OrderByDescending(s=>s.Date_Created);
                    break;
                case "DateUpdated":
                    tickets = tickets.OrderBy(s => s.Date_Updated);
                    break;
                case "dateUpdated_desc":
                    tickets = tickets.OrderByDescending(s => s.Date_Updated);
                    break;
                default:
                    break;
            }
            TempData["showDropDown"] = true;
            int pageSize = 5;
            return View(await PaginatedList<Ticket>.CreateAsync(tickets.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var projectIds = from u in _context.User_Project
                             where u.UserId == userId
                             select u.ProjectId;

            var tickets = from t in _context.Ticket
                          where projectIds.Contains(t.ProjectId)
                          select t;
            tickets = tickets.Include(t => t.Project).AsNoTracking();

            var valid = from u in _context.Ticket
                        where u.Id == id
                        select u;
            if (valid.Any())
            {
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

                // get the ticket history for the ticket

                var ticketHistory = from th in _context.Ticket_History
                                    join t in _context.Ticket
                                    on th.TicketId equals t.Id
                                    where th.TicketId == id
                                    select new Ticket_History
                                    {
                                        OldValueUserName = th.OldValueUserName,
                                        NewValueUserName = th.NewValueUserName,
                                        Date_Changed = th.Date_Changed,
                                        TicketUpdaterUserName = th.TicketUpdaterUserName
                                    };


                ViewBag.projectName = listData[0].projectName;
                TempData["ticketName"] = listData[0].ticketName;
                TempData["ticketId"] = listData[0].ticketId;
                ViewBag.ticketHistory = ticketHistory;

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
            else
            {
                return NoContent();
            }


            
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            if ((bool)TempData["showDropDown"] == false)
            {
                string name = (string)TempData["projectName"];
                int projectId = (int)TempData["projectId"];
                ViewBag.projectName = name;
                ViewBag.projectId = projectId;
                TempData["projectName"] = name;
                TempData["projectId"] = projectId;
            }
            else
            {
                TempData["showDropDown"] = true;
            }

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
                if (TempData["projectId"] != null)
                {
                    ticket.ProjectId = (int)TempData["projectId"];
                }   
                ticket.SubmitterUserName = User.Identity.Name;
                ticket.Date_Created = DateTime.Now;

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                var ticket_History = new Ticket_History()
                {
                    TicketId = ticket.Id,
                    OldValueUserName = "",
                    NewValueUserName = ticket.AssignedDeveloperUserName,
                    Date_Changed = DateTime.Now,
                    TicketUpdaterUserName = User.Identity.Name
            };
                _context.Ticket_History.Add(ticket_History);
                _context.SaveChanges();
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

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var projectIds = from u in _context.User_Project
                             where u.UserId == userId
                             select u.ProjectId;

            var tickets = from t in _context.Ticket
                          where projectIds.Contains(t.ProjectId)
                          select t;
            tickets = tickets.Include(t => t.Project).AsNoTracking();

            var valid = from u in _context.Ticket
                        where u.Id == id
                        select u;
            if (valid.Any())
            {

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
            String OldDeveloperUserName = ticketToUpdate.AssignedDeveloperUserName;

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
                var ticket_History = new Ticket_History()
                {
                    TicketId = ticket.Id,
                    OldValueUserName = OldDeveloperUserName,
                    NewValueUserName = ticket.AssignedDeveloperUserName,
                    Date_Changed = DateTime.Now,
                    TicketUpdaterUserName = User.Identity.Name
            };
                _context.Ticket_History.Add(ticket_History);
                _context.SaveChanges();
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
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var projectIds = from u in _context.User_Project
                             where u.UserId == userId
                             select u.ProjectId;

            var tickets = from t in _context.Ticket
                          where projectIds.Contains(t.ProjectId)
                          select t;
            tickets = tickets.Include(t => t.Project).AsNoTracking();

            var valid = from u in _context.Ticket
                        where u.Id == id
                        select u;
            if (valid.Any())
            {

                var ticket = await _context.Ticket
                .FirstOrDefaultAsync(m => m.Id == id);
                if (ticket == null)
                {
                    return NotFound();
                }

                return View(ticket);
            }
            else
            {
                return NoContent();
            }
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
