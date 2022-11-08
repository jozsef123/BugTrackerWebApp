using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackerWebApp.Controllers
{
    public class CommentsController : UsersController 
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Comments
        public IActionResult Index()
        {
            if (GetCurrentUserRoles().Contains("Admin"))
            {
                return View(_context.Comment.ToList());
            }
            return View(GetCurrentUserComments()); 
        }

        public IActionResult IndexForTicket(int? id)
        {
            TempData["ticketName"] = GetTicketById(id).First().Name;
            return View(GetCommentsByTicketId(id));
        }

        // GET: Comments/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = GetCommentById(id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            if (GetCurrentUserRoles().Contains("Admin"))
            {
                ViewBag.Tickets = new SelectList(GetAllTickets(), "Id", "Name");
            } else
            {
                ViewBag.Tickets = new SelectList(GetCurrentUserTickets(), "Id", "Name");
            }
            return View();
        }

        public IActionResult CreateForTicket(int? id)
        {
            TempData["ticketId"] = id;
            TempData["ticketName"] = GetTicketById(id).First().Name;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TicketId,Submitter,Message,CreatedWhen,UpdatedWhen")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Submitter = GetCurrentUser();
                comment.CreatedWhen = DateTime.Now;
                comment.UpdatedWhen = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (GetCurrentUser() == comment.Submitter)
            {
                return View(comment);
            }
            else
            {
                ViewBag.ErrorMessage = "Only Comment Submitter can edit";      // TODO: Show on current page
                return RedirectToAction(nameof(Index));
            }   
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,Submitter,Message,CreatedWhen,UpdatedWhen")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    comment.UpdatedWhen = DateTime.Now;
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
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (GetCurrentUser() == comment.Submitter || GetCurrentUserRoles().Contains("Admin"))
            {
                return View(comment);
            }
            else
            {
               ViewBag.ErrorMessage = "Only Comment Submitter or Admin can delete"; // TODO
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = GetCommentById(id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(x => x.Id == id);
        }
    }
}
