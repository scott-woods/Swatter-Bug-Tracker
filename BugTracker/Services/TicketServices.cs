using BugTracker.Data;
using BugTracker.Models;
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
    }
}
