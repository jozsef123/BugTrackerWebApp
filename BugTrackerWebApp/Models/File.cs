using System;
using System.ComponentModel.DataAnnotations;
using BugTrackerWebApp.Data;

namespace BugTrackerWebApp.Models
{
    public class File
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string SubmitterUserName { get; set; }
        public string Description { get; set; }
        public DateTime Date_Created { get; set; }
        [Required]
        public byte[] Data { get; set; }
        public Ticket Ticket { get; set; }

        public File()
        {

        }
    }
}
