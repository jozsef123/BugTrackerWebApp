using BugTrackerWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Source Used: https://github.com/T0shik/rolesvsclaimsvspolicy
namespace BugTrackerWebApp.Controllers
{
    public class UsersAccountController : UsersController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsersAccountController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            SignInManager<IdentityUser> signInManager) :base(context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var roles = _context.Roles.Where(x => x.Id != "e").ToList();        // Show all Roles but Demo
            var adminUser = _context.Users.Where(x => x.UserName == "admin@test.com").First();
            var userRoles = _context.UserRoles.ToList();

            var users = (from u in _context.Users
                        where u.UserName != "admin@test.com"
                        select u).ToList();

            // query for users that are not admin@test.com but also are not users with role of demo

            var convertedUsers = users.Select(x => new UsersViewModel
            {
                Email = x.Email,
                Roles = roles
                    .Where(y => userRoles.Any(z => z.UserId == x.Id && z.RoleId == y.Id))
                    .Select(y => new UsersRole
                    {
                        Name = y.Name
                    })
            });

            return View(new DisplayViewModel
            {
                Roles = roles.Select(x => x.Name),
                Users = convertedUsers
            });
        }

        [AllowAnonymous]
        public Tuple<IdentityUser, string> GenerateUser()
        {
            Random rnd = new();
            string username = "";
            // check that username is unique
            var foundUserInDatabase = false;
            do
            {
                username = "DemoUser-" + rnd.Next().ToString();
                foundUserInDatabase = _context.Users.Any(x => x.UserName == username);
            } while (foundUserInDatabase.Equals(true));
            string password = "Password123!";
            var user = new IdentityUser
            {
                UserName = username + "@test.com",
                Email = username + "@test.com",
            };
            return new Tuple<IdentityUser, string>(user, password);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Demo()
        {
            (var user, string password) = GenerateUser();

            await _userManager.CreateAsync(user, password);
            _userManager.AddToRoleAsync(user, "Demo").GetAwaiter().GetResult();
            await _signInManager.PasswordSignInAsync(user.Email, password, isPersistent: false, lockoutOnFailure: false);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel vm)
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = vm.Name });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleViewModel vm)
        {
            if (vm.UserEmail != null && vm.Role != null)
            {
                var user = await _userManager.FindByEmailAsync(vm.UserEmail);

                if (vm.Delete)
                    await _userManager.RemoveFromRoleAsync(user, vm.Role);
                else
                    await _userManager.AddToRoleAsync(user, vm.Role);
            }

            return RedirectToAction("Index");
        }


    }

    public class DisplayViewModel
    {
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<UsersViewModel> Users { get; set; }
    }

    public class UsersViewModel
    {
        public string Email { get; set; }
        public IEnumerable<UsersRole> Roles { get; set; }
    }

    public class UsersRole
    {
        public string Name { get; set; }
    }

    public class RoleViewModel
    {
        public string Name { get; set; }
    }

    public class UpdateUserRoleViewModel
    {
        public IEnumerable<UsersViewModel> Users { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public string UserEmail { get; set; }
        public string Role { get; set; }
        public bool Delete { get; set; }
    }
}

