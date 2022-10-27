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
    public class App_FileController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;

        public App_FileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Files
        public async Task<IActionResult> Index()
        {
            int temp = (int)TempData["ticketId"];
            string name = (string)TempData["ticketName"];
            TempData["ticketName"] = name;
            TempData["ticketId"] = temp;
            var app_File = from a in _context.App_File
                           where a.TicketId == temp
                           select a;
            ViewBag.ticketName = name;
            return View(app_File);
        }

        // GET: Files/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app_File = await _context.App_File
                .FirstOrDefaultAsync(m => m.Id == id);
            if (app_File == null)
            {
                return NotFound();
            }

            string name = (string)TempData["ticketName"];
            TempData["ticketName"] = name;
            TempData["ticketId"] = app_File.TicketId;

            ViewBag.ticketName = name;
            return View(app_File);
        }

        [HttpGet]
        public Microsoft.AspNetCore.Mvc.FileResult DownLoadFile(int id)
        {
            var FileById = (from a in _context.App_File
                            where a.Id.Equals(id)
                            select new { a.Data, a.FileName, a.FileType }).FirstOrDefault();

            string[] fileExtension = FileById.FileType.Split('/');

            return File(FileById.Data, FileById.FileType, FileById.FileName);

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
        public async Task<IActionResult> Create([Bind("Id,TicketId,SubmitterUserName,Description,Date_Created, Data, FormFile, FileName, FileType")] App_File app_File)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (memoryStream.Length < 2097152)
                    {
                        int temp = (int)TempData["ticketId"];
                        app_File.TicketId = temp;
                        app_File.SubmitterUserName = User.Identity.Name;
                        app_File.Date_Created = DateTime.Now;
                        app_File.FileName = app_File.FormFile.FileName;
                        app_File.FileType = app_File.FormFile.ContentType;
                        await app_File.FormFile.CopyToAsync(memoryStream);
                        app_File.Data = memoryStream.ToArray();
                        _context.Add(app_File);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(app_File);
        }

        // GET: Files/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.App_File.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,SubmitterUserName,Description,Date_Created, Data, FormFile, FileName, FileType")] App_File app_File)
        {
            
            if (id != app_File.Id)
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
                        app_File.TicketId = (int)TempData["ticketId"];
                        app_File.FileName = app_File.FormFile.FileName;
                        app_File.FileType = app_File.FormFile.ContentType;
                        await app_File.FormFile.CopyToAsync(memoryStream);
                        app_File.Data = memoryStream.ToArray();
                        _context.Update(app_File);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(app_File.Id))
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
            return View(app_File);
        }

        // GET: Files/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.App_File
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var file = await _context.App_File.FindAsync(id);
            _context.App_File.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(int id)
        {
            return _context.App_File.Any(e => e.Id == id);
        }
    }
}
