using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class TicketServices : ITicketServices
    {
        private readonly AppDbContext _context;

        public TicketServices(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _context.Tickets
                .Include(t => t.AssignedDeveloper)
                .Include(t => t.Creator)
                .Include(t => t.LastUpdatedBy)
                .Include(t => t.Project);
        }

        public IEnumerable<Ticket> GetAllByUser(string userId)
        {
            //Get all Tickets from DB where User is the Assigned Developer or Creator of the Ticket
            //For Managers/Admins, return all Tickets in all Projects that they manage.
            var tickets = _context.Tickets.Where(t => (t.AssignedDeveloper.Id == userId) || (t.Creator.Id == userId) || (t.Project.Creator.Id == userId))
                                .Include(t => t.AssignedDeveloper)
                                .Include(t => t.Creator)
                                .Include(t => t.LastUpdatedBy)
                                .Include(t => t.Project);
            return tickets;
        }

        public Dictionary<Priority, int> GetPriorityCount(IEnumerable<Ticket> tickets = default)
        {
            if (tickets == default)
            {
                tickets = GetAll();
            }
            var lowPriTickets = tickets.Where(t => t.TicketPriority == Priority.Low).Count();
            var medPriTickets = tickets.Where(t => t.TicketPriority == Priority.Medium).Count();
            var highPriTickets = tickets.Where(t => t.TicketPriority == Priority.High).Count();
            var priorityCount = new Dictionary<Priority, int>();
            priorityCount.Add(Priority.Low, lowPriTickets);
            priorityCount.Add(Priority.Medium, medPriTickets);
            priorityCount.Add(Priority.High, highPriTickets);

            return priorityCount;
        }

        public Dictionary<Ticket_Type, int> GetTypeCount(IEnumerable<Ticket> tickets = default)
        {
            if (tickets == default)
            {
                tickets = GetAll();
            }
            var bugsCount = tickets.Where(t => t.TicketType == Ticket_Type.Bugs).Count();
            var requestCount = tickets.Where(t => t.TicketType == Ticket_Type.Feature_Requests).Count();
            var otherCount = tickets.Where(t => t.TicketType == Ticket_Type.Other).Count();
            var typeCount = new Dictionary<Ticket_Type, int>();
            typeCount.Add(Ticket_Type.Bugs, bugsCount);
            typeCount.Add(Ticket_Type.Feature_Requests, requestCount);
            typeCount.Add(Ticket_Type.Other, otherCount);

            return typeCount;
        }

        public Dictionary<Status, int> GetStatusCount(IEnumerable<Ticket> tickets = default)
        {
            if (tickets == default)
            {
                tickets = GetAll();
            }
            var openCount = tickets.Where(t => t.TicketStatus == Status.Open).Count();
            var inprogCount = tickets.Where(t => t.TicketStatus == Status.In_Progress).Count();
            var resolvedCount = tickets.Where(t => t.TicketStatus == Status.Resolved).Count();
            var statusCount = new Dictionary<Status, int>();
            statusCount.Add(Status.Open, openCount);
            statusCount.Add(Status.In_Progress, inprogCount);
            statusCount.Add(Status.Resolved, resolvedCount);

            return statusCount;
        }

        public IEnumerable<Ticket> GetAllByProjectId(int projectId)
        {
            return _context.Tickets.Where(t => t.Project.Id == projectId)
                .Include(t => t.AssignedDeveloper)
                .Include(t => t.Creator)
                .Include(t => t.LastUpdatedBy)
                .Include(t => t.Project);      
        }

        public void Add(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        public Ticket GetById(int id)
        {
            return _context.Tickets.Where(t => t.Id == id)
                .Include(t => t.Project)
                .Include(t => t.AssignedDeveloper)
                .Include(t => t.Creator)
                .Include(t => t.LastUpdatedBy)
                .FirstOrDefault();
        }

        public TicketListingModel FormatTicket(Ticket ticket)
        {
            var listingResult = new TicketListingModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Project = ticket.Project,
                AssignedDeveloper = ticket.AssignedDeveloper,
                CreateDate = ticket.CreateDate,
                Creator = ticket.Creator,
                LastUpdateDate = ticket.LastUpdateDate,
                LastUpdatedBy = ticket.LastUpdatedBy,
                TicketPriority = ticket.TicketPriority,
                TicketStatus = ticket.TicketStatus,
                TicketType = ticket.TicketType,
                Comments = _context.Comments.Where(c => c.Ticket.Id == ticket.Id).ToList()
            };

            return listingResult;
        }
    }
}
