using BugTrackerWebApp.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace BugTrackerWebApp.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string PreviousAssignedDeveloperId { get; set; }
        public IdentityUser PreviousAssignedDeveloper { get; set; }
        public string NewAssignedDeveloperId { get; set; }
        public IdentityUser NewAssignedDeveloper { get; set; }
        public DateTime UpdatedWhen { get; set; }
        public IdentityUser Updater { get; set; }
        public Ticket Ticket { get; set; }
        public TicketHistory(){}
    }
}
