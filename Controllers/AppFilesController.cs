using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerWebApp.Controllers
{
    public class AppFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Files
        public IActionResult Index()
        {
            int ticketId = (int)TempData["ticketId"];       // TODO: Try to avoid doing this TempData repeately
            string ticketName = (string)TempData["ticketName"];
            TempData["ticketName"] = ticketName;
            TempData["ticketId"] = ticketId;
            var appFile = from aF in _context.AppFile
                           where aF.Ticket.Id == ticketId
                           select aF;
            // TODO what happens where there are no files in the app? 
            // Do we want to show something?
            ViewBag.ticketName = ticketName;
            return View(appFile);
        }

        // GET: Files/Details/5
        public IActionResult Details(int? id)
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
                return NotFound(); // TODO show an error page (also test out and see what it says)
            }

            string ticketName = (string)TempData["ticketName"];         // TODO: Revise
            TempData["ticketName"] = ticketName;
            TempData["ticketId"] = appFile.Ticket.Id;

            ViewBag.ticketName = ticketName;
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

        // GET: Files/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Submitter,Description,CreatedWhen, Data, FormFile, Name, Type")] AppFile appFile)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (memoryStream.Length < 2097152)
                    {
                        int temp = (int)TempData["ticketId"]; // TODO
                        // appFile.TicketId = temp;
                        appFile.Submitter = (from u in _context.Users
                                                               where u.UserName == User.Identity.Name
                                                               select u).First();
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

        // GET: Files/Edit/5
        public IActionResult Edit(int? id)
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

        // POST: Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Submitter,Description,CreatedWhen, Data, FormFile, Name, Type")] AppFile appFile)
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
                        var ticketId = (int)TempData["ticketId"]; // Revise later
                        var ticket = (from t in _context.Ticket
                                     where t.Id == ticketId
                                      select t).First();
                        appFile.Ticket = ticket; 
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

        // GET: Files/Delete/5
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

        // POST: Files/Delete/5
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
