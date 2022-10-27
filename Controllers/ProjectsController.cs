using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace BugTrackerWebApp.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            List<int> projectIDs = new List<int>();
            // https://stackoverflow.com/questions/38543193/proper-way-to-get-current-user-id-in-entity-framework-core
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            // get all project id's from User_Project that are joined to the user id
            projectIDs = _context.User_Project.Where(p=>p.UserId == userId).Select(p => p.ProjectId).ToList();

            // Source: https://stackoverflow.com/questions/27372187/how-to-get-name-from-another-table-with-id-mvc-5
            // get the role id that matches user id in UserRoles table
            var roleId = from u in _context.UserRoles
                         where u.UserId == userId
                         select u.RoleId;
            // get the role name in Roles table that matches the role id from the UserRoles table 
            if (roleId.Count() > 0)
            {
                var roleName = from r in _context.Roles
                                  where roleId.Contains(r.Id)
                                  select r.Name;
                if (roleName.Contains("Admin"))
                {
                    return View(await _context.Project.ToListAsync());
                }
            }

            var projects = _context.Project
                .Where(x => projectIDs.Contains(x.Id))
                .AsNoTracking();
            
            return View(await projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // check to make sure current user has project in user table
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var userProjects = from u in _context.User_Project
                        where u.UserId == userId
                        select u;
            if (userProjects.Count() > 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var project = await _context.Project
                    .Include(p => p.Tickets)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (project == null)
                {
                    return NotFound();
                }
                TempData["projectName"] = project.Name;
                TempData["projectId"] = project.Id;

                TempData["showDropDown"] = false;                       // if user creates a new ticket, show Project Name

                return View(project);
            }
            else
            {
                return NoContent();
            }
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                var name = _context.Project.FirstOrDefault(x=>x.Name == project.Name);
                if (name == null)
                {
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    User_Project user_Project = new User_Project();
                    user_Project.ProjectId = project.Id;
                    user_Project.UserId = (from u in _context.Users
                                           where u.UserName == User.Identity.Name
                                           select u.Id ).FirstOrDefault().ToString();

                    user_Project.UserName = User.Identity.Name;
                    _context.Add(user_Project);
                    await _context.SaveChangesAsync();



                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = "Duplicate Project Name. Enter a Unique Project Name.";
                }
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var userProjects = from u in _context.User_Project
                               where u.UserId == userId
                               select u;
            if (userProjects.Count() > 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var project = await _context.Project.FindAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                return View(project);
            }
            else
            {
                return NoContent();
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
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
        public async Task<IActionResult> Delete(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;       // current user ID
            var userProjects = from u in _context.User_Project
                               where u.UserId == userId
                               select u;
            if (userProjects.Count() > 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var project = await _context.Project
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (project == null)
                {
                    return NotFound();
                }

                return View(project);
            }
            else
            {
                return NoContent();
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
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
