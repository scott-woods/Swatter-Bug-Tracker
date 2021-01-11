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
