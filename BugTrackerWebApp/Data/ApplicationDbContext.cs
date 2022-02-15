using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using BugTrackerWebApp.Models;

namespace BugTrackerWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BugTrackerWebApp.Models.Ticket> Ticket { get; set; }
        public DbSet<BugTrackerWebApp.Models.Project> Project { get; set; }
        public DbSet<BugTrackerWebApp.Models.Ticket_History> Ticket_History { get; set; }
        public DbSet<BugTrackerWebApp.Models.File> File { get; set; }
        public DbSet<BugTrackerWebApp.Models.Comment> Comment { get; set; }
        public DbSet<BugTrackerWebApp.Models.User_Project> User_Project { get; set; }
    }
}
