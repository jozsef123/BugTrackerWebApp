using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BugTrackerWebApp.Data;
using BugTrackerWebApp.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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


            }

            host.Run();        

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var builtConfiguration = config.Build();

                string kvURL = builtConfiguration["KeyVaultConfig:KVUrl"];
                string tenantId = builtConfiguration["KeyVaultConfig:TenantId"];
                string clientId = builtConfiguration["KeyVaultConfig:ClientId"];
                string clientSecret = builtConfiguration["KeyVaultConfig:ClientSecretId"];

                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

                var client = new SecretClient(new System.Uri(kvURL), credential);
                config.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());

            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
