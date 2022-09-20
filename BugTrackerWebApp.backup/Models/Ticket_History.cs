using BugTrackerWebApp.Data;
using System;

namespace BugTrackerWebApp.Models
{
    public class Ticket_History
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string OldValueUserName { get; set; }
        public string NewValueUserName { get; set; }
        public DateTime Date_Changed { get; set; }
        public string TicketUpdaterUserName { get; set; }
        public Ticket Ticket { get; set; }

        public Ticket_History()
        {

        }
    }
}
