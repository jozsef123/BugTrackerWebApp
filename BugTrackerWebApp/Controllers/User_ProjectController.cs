using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace BugTrackerWebApp.Controllers
{
    public class User_ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;


        private string connectionString = @"Server=(localdb)\\mssqllocaldb;
                         Database=aspnet-BugTrackerWebApp-BCBCC5F0-00DD-4462-88CA-62FECF41A6C8;
                        Trusted_Connection=True;MultipleActiveResultSets=true";
        public User_ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User_Project
        public async Task<IActionResult> Index()
        {
            var user_projects = _context.User_Project
                .Include(p => p.Project)
                .AsNoTracking();
            TempData["projectId"] = null;
            return View(await user_projects.ToListAsync());

        }

        // GET: User_Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
     
            string sqlQuery = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                sqlQuery = "select u.Username from Users as u, User_Project as up " +
                    "where u.Id = up.UserId";
            }

            var user_Project = await _context.User_Project
                .Include(p=>p.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user_Project == null)
            {
                return NotFound();
            }

            return View(user_Project);
        }

        // GET: User_Project/Create
        public IActionResult Create()
        {
            // query for all users that are not in the project
            var users = _context.Users
                .Where(u => !_context.User_Project
                .Select(up => up.UserId)
                .Contains(u.Id));
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            // see if the user was on Project/Details before clicking on add a user to project (User_Project/Create)
            if (TempData["projectId"] != null)
            {
                string name = (string)TempData["projectName"];
                int projectId = (int)TempData["projectId"];
                ViewBag.projectName = name;
                ViewBag.projectId = projectId;
                ViewBag.showDropDown = false;
                TempData["projectName"] = name;
                TempData["projectId"] = projectId;
            }
            else {
                ViewBag.showDropDown = true;
            }
            return View();
        }

        // POST: User_Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId, UserId, UserName")] User_Project user_Project)
        {
           
            user_Project.UserName = (from u in _context.Users
                                     where u.Id == user_Project.UserId
                                     select u.UserName).Single();
            user_Project.ProjectId = (int)TempData["projectId"];
            user_Project.Project = (from p in _context.Project
                                    where p.Id == user_Project.ProjectId
                                    select p).Single();
            var recordExists = from p in _context.User_Project
                               where p.UserId == user_Project.UserId && p.ProjectId == user_Project.ProjectId
                               select p;
            if (ModelState.IsValid  && recordExists.Count() == 0)
            {
                _context.Add(user_Project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("Existing record");
                TempData["Error"] = "User and project already exist in database";
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: User_Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var user_Project = await _context.User_Project
                .Include(p => p.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_Project == null)
            {
                return NotFound();
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.Projects = new SelectList(_context.Project, "Id", "Name");
            return View(user_Project);
        }

        // POST: User_Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,UserName, ProjectId")] User_Project user_Project)
        {
            if (id != user_Project.Id)
            {
                return NotFound();
            }

            user_Project.UserName = (from u in _context.Users
                                     where u.Id == user_Project.UserId
                                     select u.UserName).Single();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user_Project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!User_ProjectExists(user_Project.Id))
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
            return View(user_Project);
        }

        // GET: User_Project/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_Project = await _context.User_Project
                .Include(x => x.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_Project == null)
            {
                return NotFound();
            }

            return View(user_Project);
        }

        // POST: User_Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user_Project = await _context.User_Project.FindAsync(id);
            _context.User_Project.Remove(user_Project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_ProjectExists(int id)
        {
            return _context.User_Project.Any(e => e.Id == id);
        }
    }
}
