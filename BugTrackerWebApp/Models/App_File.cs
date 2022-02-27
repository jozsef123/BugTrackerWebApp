using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Threading.Tasks;

namespace BugTrackerWebApp.Models
{
    public class App_File
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string SubmitterUserName { get; set; }
        public string Description { get; set; }
        public DateTime Date_Created { get; set; }
        public byte[] Data { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Ticket Ticket { get; set; }


        public App_File()
        {

        }

        internal Task CopyToAsync(MemoryStream memoryStream)
        {
            throw new NotImplementedException();
        }
    }
}
