using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BugTrackerWebApp.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public IdentityUser Owner { get; set; }
        public DateTime CreatedWhen { get; } = DateTime.Now;
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<UserProject> UserProjects { get; set; }
        public Project(){}
    }
}
