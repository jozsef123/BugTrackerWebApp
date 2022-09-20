using BugTrackerWebApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var services = host.Services.CreateScope())
            {
                var dbContext = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userMgr = services.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleMgr = services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                dbContext.Database.Migrate();

                var adminRole = new IdentityRole("Admin")
                {
                    Id = "a"
                };
                var projectManagerRole = new IdentityRole("Project Manager")
                {
                    Id = "b"
                };
                var developerRole = new IdentityRole("Developer")
                {
                    Id = "c"
                };
                var submitterRole = new IdentityRole("Submitter")
                {
                    Id = "d"
                };

                if (!dbContext.Roles.Any())
                {
                    roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(projectManagerRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(developerRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(submitterRole).GetAwaiter().GetResult();
                    
                }

                if (!dbContext.Users.Any(u => u.UserName == "admin@test.com"))
                {
                    var adminUser = new IdentityUser
                    {
                        UserName = "admin@test.com",
                        Email = "admin@test.com"
                    };
                    var result = userMgr.CreateAsync(adminUser, "Password123!").GetAwaiter().GetResult();
                    userMgr.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
                }

                // demo user with role of admin
                if (!dbContext.Users.Any(u => u.UserName == "demo_admin@test.com"))
                {
                    var demoAdminUser = new IdentityUser
                    {
                        UserName = "demo_admin@test.com",
                        Email = "demo_admin@test.com"
                    };
                    var result = userMgr.CreateAsync(demoAdminUser, "Password123!").GetAwaiter().GetResult();
                    userMgr.AddToRoleAsync(demoAdminUser, adminRole.Name).GetAwaiter().GetResult();
                }
                // demo user with role of project manager
                if (!dbContext.Users.Any(u => u.UserName == "demo_projectmanager@test.com"))
                {
                    var demoProjectManagerUser = new IdentityUser
                    {
                        UserName = "demo_projectmanager@test.com",
                        Email = "demo_projectmanager@test.com"
                    };
                    var result = userMgr.CreateAsync(demoProjectManagerUser, "Password123!").GetAwaiter().GetResult();
                    userMgr.AddToRoleAsync(demoProjectManagerUser, projectManagerRole.Name).GetAwaiter().GetResult();
                }
                // demo user with role of developer
                if (!dbContext.Users.Any(u => u.UserName == "demo_developer@test.com"))
                {
                    var demoDeveloperUser = new IdentityUser
                    {
                        UserName = "demo_developer@test.com",
                        Email = "demo_developer@test.com"
                    };
                    var result = userMgr.CreateAsync(demoDeveloperUser, "Password123!").GetAwaiter().GetResult();
                    userMgr.AddToRoleAsync(demoDeveloperUser, developerRole.Name).GetAwaiter().GetResult();
                }
                // demo user with role of submitter
                if (!dbContext.Users.Any(u => u.UserName == "demo_submitter@test.com"))
                {
                    var demoSubmitterUser = new IdentityUser
                    {
                        UserName = "demo_submitter@test.com",
                        Email = "demo_submitter@test.com"
                    };
                    var result = userMgr.CreateAsync(demoSubmitterUser, "Password123!").GetAwaiter().GetResult();
                    userMgr.AddToRoleAsync(demoSubmitterUser, submitterRole.Name).GetAwaiter().GetResult();
                }

            }

            host.Run();
        
    }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://bug-tracker-web-app-jozsef.herokuapp.com/");
                });
    }
}
