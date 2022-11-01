using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BugTrackerWebApp.Models;

namespace BugTrackerWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<TicketHistory> TicketHistory { get; set; }
        public DbSet<AppFile> AppFile { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<UserProject> UserProject { get; set; }
    }
}
