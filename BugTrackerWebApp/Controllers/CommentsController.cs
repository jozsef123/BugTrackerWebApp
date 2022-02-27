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
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var ticketName = TempData["ticketName"];
            var ticketId = (int)TempData["ticketId"];
            TempData["ticketId"] = ticketId;
            TempData["ticketName"] = ticketName;
            //var comments = _context.Comment
            //   .Include(t => t.Ticket)
            //   .AsNoTracking();
            var comments = from c in _context.Comment
                           where c.TicketId == ticketId
                           select c;
            ViewBag.ticketName = ticketName;
            return View(await comments.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TicketId,SubmitterUserName,Message,Date_Created")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                int temp = (int)TempData["ticketId"];
                comment.TicketId = temp;
                comment.SubmitterUserName = User.Identity.Name;
                comment.Date_Created = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var comment = await _context.Comment
                .Include(c=>c.Ticket)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            var currentUser = User.Identity.Name;

            if (currentUser == comment.SubmitterUserName)
            {
                if (comment == null)
                {
                    return NotFound();
                }
                return View(comment);
            }
            else
            {
                Console.WriteLine("Only Comment Submitter can edit");
                TempData["Error"] = "Only Comment Submitter can edit";
                return RedirectToAction(nameof(Index));
            }   
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,SubmitterUserName,Message,Date_Created")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }
            int temp = (int)TempData["ticketId"];
            comment.TicketId = temp;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserName = User.Identity.Name;
            var currentUser = from u in _context.Users
                                  where u.UserName == currentUserName
                                select u.Id.SingleOrDefault();

            var currentUserId = _context.Users.Where(z => z.UserName == currentUserName).Select(z=>z.Id).SingleOrDefault();

            var currentRoles = _context.UserRoles.Where(z=>z.UserId == currentUserId).Select(z=>z.RoleId).ToList();

            var currentRoleName = _context.Roles.Select(z=>z.NormalizedName).ToList();


            
            Console.WriteLine(currentRoleName[0]);

            if (currentUserName == comment.SubmitterUserName || currentRoleName.Contains("ADMIN"))
            {
                if (comment == null)
                {
                    return NotFound();
                }
                return View(comment);
            }
            else
            {
                Console.WriteLine("Only Comment Submitter or Admin can delete");
                TempData["Error"] = "Only Comment Submitter or Admin can delete";
                return RedirectToAction(nameof(Index));
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
