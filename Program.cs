using BugTrackerWebApp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace BugTrackerWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
                var demoRole = new IdentityRole("Demo")
                {
                    Id = "e"
                };

                if (!dbContext.Roles.Any())
                {
                    roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(projectManagerRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(developerRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(submitterRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(demoRole).GetAwaiter().GetResult();
                    
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
            }
   
            var app = host;
            var scope = app.Services.CreateScope();
            DataHelper.ManageDataAsync(scope.ServiceProvider).Wait();
            var builder = WebApplication.CreateBuilder(args);
            var services1 = builder.Services;
            var configuration = builder.Configuration;

            services1.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Environment.GetEnvironmentVariable("GOOGLE__CLIENTID");
                googleOptions.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE__CLIENTSECRET");
            });
            host.Run();        

        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            DotNetEnv.Env.Load();
            return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}
