using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerWebApp.Models
{
    public enum Priority
    {
        Low, Medium, High
    }

    // https://community.atlassian.com/t5/Jira-articles/Understanding-issue-types-in-jira/ba-p/1497237
    public enum Type
    {
        Story, Task, Bug, Subtask, Epic 
    }

    public enum Status
    {
        Open, Closed, 
    }
    public class Ticket
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public IdentityUser Submitter { get; set; }
        public string AssignedDeveloperId { get; set; }
        public IdentityUser AssignedDeveloper { get; set; }
        public Priority? Priority { get; set; }
        public Type? Type { get; set; }       
        public Status? Status { get; set; }
        public DateTime CreatedWhen { get; set; }
        public DateTime? UpdatedWhen { get; set; }
        public Project Project { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AppFile> AppFiles { get; set; }
        public ICollection<TicketHistory> TicketHistory { get; set; }
        public Ticket(){}
    }
}
