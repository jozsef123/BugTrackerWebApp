using Microsoft.AspNetCore.Identity;

namespace BugTrackerWebApp.Models
{
    public class UserProject
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public UserProject()
        {

        }
    }
}
