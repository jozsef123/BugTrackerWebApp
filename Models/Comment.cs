using Microsoft.AspNetCore.Identity;
using System;

namespace BugTrackerWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public IdentityUser Submitter { get; set; }
        public string Message { get; set; }
        public DateTime CreatedWhen { get; set; }
        public DateTime? UpdatedWhen { get; set; }
        public Ticket Ticket { get; set; }
        public Comment(){}
    }
}
