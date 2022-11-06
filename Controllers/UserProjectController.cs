using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;

namespace BugTrackerWebApp.Controllers
{
    public class UserProjectController : UsersController
    {
        private readonly ApplicationDbContext _context;

        public UserProjectController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: UserProject
        public IActionResult Index()
        {
            var userProjects = GetAllUserProjects();
            return View(userProjects);

        }

        // GET: UserProject/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
     
            var userProject = GetUserProjectById(id);

            if (userProject == null)
            {
                return NotFound();
            }

            return View(userProject);
        }

        // GET: UserProject/Create
        public IActionResult Create()
        {
            var users = GetNonDemoUsers().ToList();
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            return View();
        }

        // POST: UserProject/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId, UserId")] UserProject userProject)
        {
            userProject.User = GetUserById(userProject.UserId);
            userProject.Project = GetProjectById(userProject.ProjectId);
            var recordExists = (from p in _context.UserProject
                               where p.User.Id == userProject.User.Id && p.Project.Id == userProject.Project.Id
                               select p).Any();
            if (ModelState.IsValid && recordExists == false)
            {
                _context.Add(userProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            ViewBag.ErrorMessage = "User and project already exist in database";
            return View(userProject);
        }

        // GET: UserProject/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var userProject = GetUserProjectById(id);
            if (userProject == null)
            {
                return NotFound();
            }
            var users = GetNonDemoUsers().ToList();
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            return View(userProject);
        }

        // POST: UserProject/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId, UserId")] UserProject userProject)
        {
            if (id != userProject.Id)
            {
                return NotFound();
            }

            userProject.User = GetUserById(userProject.UserId);
            userProject.Project = GetProjectById(userProject.ProjectId);
            var recordExists = (from p in _context.UserProject
                               where p.User.Id == userProject.User.Id && p.Project.Id == userProject.Project.Id
                               select p).Any();

            if (ModelState.IsValid && recordExists == false)
            {
                try
                {
                    _context.Update(userProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProjectExists(userProject.Id))
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
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            ViewBag.ErrorMessage = "User and project already exist in database";
            return View(userProject);
        }

        // GET: UserProject/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProject = GetUserProjectById(id);
            if (userProject == null) return NotFound();

            return View(userProject);
        }

        // POST: UserProject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProject = await _context.UserProject.FindAsync(id);
            _context.UserProject.Remove(userProject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProjectExists(int id)
        {
            return _context.UserProject.Any(e => e.Id == id);
        }
    }
}
