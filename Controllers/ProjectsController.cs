using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BugTrackerWebApp.Controllers
{
    public class ProjectsController : UsersController
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Projects
        // Admin or project member have access
        public IActionResult Index()
        {
            if (GetCurrentUserRoles().Contains("Admin"))
            {
                return View(_context.Project.Include(p => p.Owner).ToList());
            }
            
            return View(GetCurrentUserProjects());
        }

        // GET: Projects/Details/5
        // Admin or project member have access
        public IActionResult DetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = GetProjectById(id);

            if (project == null) return NotFound();

            if (GetCurrentUserRoles().Contains("Admin") || GetCurrentUserProjectsId().Contains(project.Id))
            {
                return View(project);
            }
            return NoContent();
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,OwnerId,CreatedWhen")] Project project)
        {
            var name = _context.Project.FirstOrDefault(x => x.Name == project.Name);
            if (project.Name.Contains(' '))
            {
                ViewBag.ErrorMessage = "No spaces allowed in a Project Name.";
            }
            else if (name != null)
            {
                ViewBag.ErrorMessage = "Duplicate Project Name. Enter a Unique Project Name.";
            }
            else if (ModelState.IsValid)
            {
                project.OwnerId = GetCurrentUser().Id;
                project.CreatedWhen = DateTime.Now;
                _context.Add(project);
                await _context.SaveChangesAsync();
                UserProject user_Project = new()
                {
                    Project = project,
                    User = GetCurrentUser()
                };
                _context.Add(user_Project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        // Only admin or project members have access
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var project = GetProjectById(id);
            if (project == null) return NotFound();
            if (GetCurrentUserRoles().Contains("Admin") || GetCurrentUserProjectsId().Contains(project.Id))
            {
                ViewBag.Users = new SelectList(GetUsersInProject(project), "Id", "UserName");
                return View(project);
            }
            return NoContent();
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,OwnerId,CreatedWhen")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }
            // only existing project owner can change project owner
            var differentProjectAlreadyHasName = (from p in _context.Project
                                                  where p.Name == project.Name && p.Id != project.Id
                                                  select p).FirstOrDefault();

            if (project.Name.Contains(' '))
            {
                ViewBag.ErrorMessage = "No spaces allowed in a Project Name.";
            }
            else if (differentProjectAlreadyHasName != null)
            {
                ViewBag.ErrorMessage = "Duplicate Project Name. Enter a Unique Project Name.";
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        // Only admin or project members have access
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var project = GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            var userProjectsId = GetCurrentUserProjectsId();

            if (GetCurrentUserRoles().Contains("Admin") || userProjectsId.Contains(project.Id))
            {
                return View(project);
            }
            return NoContent();
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = GetProjectById(id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
