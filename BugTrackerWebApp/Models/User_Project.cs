
using BugTrackerWebApp.Data;

namespace BugTrackerWebApp.Models
{
    public class User_Project
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public User_Project()
        {

        }
    }
}
