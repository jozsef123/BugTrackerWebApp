using BugTrackerWebApp.Data;
using System;

namespace BugTrackerWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string SubmitterUserName { get; set; }
        public string Message { get; set; }
        public DateTime Date_Created { get; set; }

        public Ticket Ticket { get; set; }
        public Comment()
        {

        }
    }
}
