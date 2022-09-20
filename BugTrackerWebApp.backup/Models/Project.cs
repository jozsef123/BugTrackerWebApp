using System.Collections.Generic;

namespace BugTrackerWebApp.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<User_Project> User_Projects { get; set; }

        public Project()
        {

        }
    }
}
