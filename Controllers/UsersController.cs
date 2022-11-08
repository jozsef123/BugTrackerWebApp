using BugTrackerWebApp.Data;
using BugTrackerWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using Project = BugTrackerWebApp.Models.Project;

namespace BugTrackerWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IdentityUser GetCurrentUser()
        {
            var currentUser = (from u in _context.Users
                               where u.UserName == User.Identity.Name
                               select u).First();
            return currentUser;
        }

        public List<string> GetCurrentUserRoles()
        {
            var roleIds = (from u in _context.UserRoles
                         where u.UserId == GetCurrentUser().Id
                         select u.RoleId).ToList();
            var roleNames = (from r in _context.Roles
                           where roleIds.Contains(r.Id)
                           select r.Name).ToList();
            return roleNames;
        }

        public IQueryable<Project> GetCurrentUserProjects()
        {
            var projects = (from up in _context.UserProject 
                                where up.User.Id == GetCurrentUser().Id
                                select up.Project);
            return projects;
        }

        public List<int> GetCurrentUserProjectsId()
        {
            var ids = (from up in _context.UserProject
                            where up.User.Id == GetCurrentUser().Id
                            select up.Project.Id).ToList();
            return ids;
        }

        public Project GetProjectById(int? id)
        {
            var project = (from p in _context.Project
                           where p.Id == id
                           select p)
                           .Include(p => p.Tickets)
                           .Include(p => p.Owner)
                           .First();
            return project;
        }

        public IQueryable<Ticket> GetCurrentUserTickets()
        {
            var tickets = (from t in _context.Ticket
                          where GetCurrentUserProjectsId().Contains(t.Project.Id)
                          select t)
                          .Include(t => t.Project)
                          .Include(t => t.Submitter)
                          .Include(t => t.AssignedDeveloper)
                          .Include(t => t.Comments)
                          .Include(t => t.AppFiles)
                          .Include(t => t.TicketHistory);
            return tickets;
        }

        public IQueryable<int> GetCurrentUserTicketsId()
        {
            var ticketsId = (from t in _context.Ticket
                           where GetCurrentUserProjectsId().Contains(t.Project.Id)
                           select t.Id);
            return ticketsId;
        }

        public IIncludableQueryable<Ticket, ICollection<AppFile>> GetTicketById(int? id)
        {
            var ticket =  (from t in _context.Ticket
                          where t.Id == id
                          select t)
                         .Include(t => t.Comments)
                         .Include(t => t.Project)
                         .Include(t => t.TicketHistory)
                         .Include(t => t.Submitter)
                         .Include(t => t.AppFiles);
            return ticket;
        }

        public bool IsUserTicket(Ticket ticket)
        {
            return GetCurrentUserProjectsId().Contains(ticket.Project.Id);
        }

        public IQueryable<IdentityUser> GetUsersInCurrentUserProjects()
        {
            return (from u in _context.UserProject
             where GetCurrentUserProjects().Contains(u.Project)
             select u.User).Distinct();
        }

        public bool IsUserInProject(string userId, int projectId)
        {
            return (from u in _context.UserProject
                        where u.User.Id == userId && u.Project.Id == projectId
                        select u).Any();
        }

        public IQueryable<IdentityUser> GetUsersInProject(Project project)
        {
            return (from u in _context.UserProject
                    where u.Project.Id == project.Id
                    select u.User).Distinct();
        }

        public IdentityUser GetUserByUserName(string username)
        {
            return (from u in _context.Users
                    where u.UserName == username
                    select u).First();
        }

        public IQueryable<UserProject> GetAllUserProjects()
        {
            return _context.UserProject.Include(u => u.Project).Include(u => u.User);
        }

        public UserProject GetUserProjectById(int? id)
        {
            return (from u in _context.UserProject
                    where u.Id == id
                    select u).Include(u => u.Project).Include(u => u.User).First();
        }

        public IdentityUser GetUserById(string id)
        {
            return (from u in _context.Users
                    where u.Id == id
                    select u).First();
        }

        public IQueryable<Comment> GetCurrentUserComments()
        {
            var comments = (from c in _context.Comment
                           where GetCurrentUserTicketsId().Contains(c.TicketId)
                           select c)
                          .Include(c => c.Submitter)
                          .Include(c => c.Ticket)
                          ;
            return comments;

        }

        public Comment GetCommentById(int? id)
        {
            var comment = (from c in _context.Comment
                           where c.Id == id
                           select c).Include(c => c.Submitter).Include(c=>c.Ticket).First();
            return comment;
        }

        public IQueryable<Comment> GetCommentsByTicketId(int? id)
        {
            var comments = (from c in _context.Comment
                            where c.TicketId == id
                            select c);
            return comments;
        }

        public IQueryable<Ticket> GetAllTickets()
        {
            var tickets = (from t in _context.Ticket
                           select t)
                          .Include(t => t.Project)
                          .Include(t => t.Submitter)
                          .Include(t => t.AssignedDeveloper)
                          .Include(t => t.Comments)
                          .Include(t => t.AppFiles)
                          .Include(t => t.TicketHistory);
            return tickets;
        }

        public IQueryable<IdentityUser> GetNonDemoUsers()
        {
            var users = (from u in _context.Users
                         join r in _context.UserRoles on u.Id equals r.UserId
                         where r.RoleId != "e"
                         select u);
            return users;
        }

        public IQueryable<AppFile> GetAppFilesByTicketId(int? id)
        {
            var files = (from f in _context.AppFile
                         where f.TicketId == id
                         select f);
            return files;
        }
    }
}
