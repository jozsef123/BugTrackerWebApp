using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerWebApp.Controllers
{
    public class AppFilesController : UsersController
    {
        private readonly ApplicationDbContext _context;

        public AppFilesController(ApplicationDbContext context) :base(context)
        {
            _context = context;
        }

        // GET: AppFiles
        public IActionResult Index()
        {
            var appFile = from f in _context.AppFile
                           where GetCurrentUserTicketsId().Contains(f.TicketId)
                           select f;
            return View(appFile);
        }

        // GET: AppFiles/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appFile = (from f in _context.AppFile
                           where f.Id == id
                           select f).Include(f => f.Ticket).Include(f => f.Submitter).First();

            if (appFile == null)
            {
                return NotFound(); // TODO show an error page (also test out and see what it says)
            }
            return View(appFile);
        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            var appFile = (from aF in _context.AppFile
                            where aF.Id == id
                            select new { aF.Data, aF.Name, aF.Type }).FirstOrDefault();

            string[] fileExtension = appFile.Type.Split('/');

            return File(appFile.Data, appFile.Type, appFile.Name);

        }

        // GET: AppFiles/Create
        public IActionResult Create()
        {
            if (GetCurrentUserRoles().Contains("Admin"))
            {
                ViewBag.Tickets = new SelectList(GetAllTickets(), "Id", "Name");
            }
            else
            {
                ViewBag.Tickets = new SelectList(GetCurrentUserTickets(), "Id", "Name");
            }
            return View();
        }

        // POST: AppFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TicketId,Submitter,Description,CreatedWhen,UpdatedWhen,Data,FormFile,Name,Type")] AppFile appFile)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (memoryStream.Length < 2097152)
                    {
                        appFile.Submitter = GetCurrentUser();
                        appFile.CreatedWhen = DateTime.Now;
                        appFile.Name = appFile.FormFile.FileName;
                        appFile.Type = appFile.FormFile.ContentType;
                        await appFile.FormFile.CopyToAsync(memoryStream);
                        appFile.Data = memoryStream.ToArray();
                        _context.Add(appFile);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appFile);
        }

        // GET: AppFiles/Edit/5
        public IActionResult Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var appFile = (from f in _context.AppFile
                          where f.Id == id
                          select f).Include(f => f.Ticket).Include(f => f.Submitter).First();
            if (appFile == null)
            {
                return NotFound();
            }

            return View(appFile);
        }

        // POST: AppFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,Submitter,Description,CreatedWhen,UpdatedWhen,Data,FormFile,Name,Type")] AppFile appFile)
        {
            if (id != appFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    if (memoryStream.Length < 2097152)
                    {
                        appFile.UpdatedWhen = DateTime.Now;
                        appFile.Name = appFile.FormFile.FileName;
                        appFile.Type = appFile.FormFile.ContentType;
                        await appFile.FormFile.CopyToAsync(memoryStream);
                        appFile.Data = memoryStream.ToArray();
                        _context.Update(appFile);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(appFile.Id))
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
            return View(appFile);
        }

        // GET: AppFiles/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appFile = (from aF in _context.AppFile
                        where aF.Id == id
                        select aF).First();
            if (appFile == null)
            {
                return NotFound();
            }

            return View(appFile);
        }

        // POST: AppFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appFile = (from aF in _context.AppFile
                           where aF.Id == id
                           select aF).First();
            _context.AppFile.Remove(appFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(int id)
        {
            return _context.AppFile.Any(x => x.Id == id);
        }
    }
}
