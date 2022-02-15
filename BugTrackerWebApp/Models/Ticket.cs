using BugTrackerWebApp.Data;
using System;
using System.Collections.Generic;

namespace BugTrackerWebApp.Models
{
    public enum Priority
    {
        Low, Medium, High
    }

    //https://community.atlassian.com/t5/Jira-articles/Understanding-issue-types-in-jira/ba-p/1497237
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
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubmitterUserName { get; set; }
        public string AssignedDeveloperUserName { get; set; }
        public Priority? Priority { get; set; }
        public Type? Type { get; set; }       
        public Status? Status { get; set; }
        public DateTime? Date_Created { get; set; }
        public DateTime? Date_Updated { get; set; }

        public Project Project { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Ticket_History> Ticket_Histories { get; set; }

        public Ticket()
        {

        }
    }
}
