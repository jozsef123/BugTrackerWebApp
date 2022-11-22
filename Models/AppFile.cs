using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Threading.Tasks;

namespace BugTrackerWebApp.Models
{
    public class AppFile
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public IdentityUser Submitter { get; set; }
        public string Description { get; set; }
        public DateTime CreatedWhen { get; } = DateTime.Now;
        public byte[] Data { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime? UpdatedWhen { get; set; }
        public Ticket Ticket { get; set; }

        public AppFile()
        {

        }

        internal Task CopyToAsync(MemoryStream memoryStream)
        {
            throw new NotImplementedException();
        }
    }
}
