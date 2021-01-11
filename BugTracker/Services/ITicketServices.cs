using BugTracker.Models;
using BugTracker.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public interface ITicketServices
    {
        public IEnumerable<Ticket> GetAll();
        public IEnumerable<Ticket> GetAllByProjectId(int projectId);
        public void Add(Ticket ticket);
        public Ticket GetById(int id);
        public TicketListingModel FormatTicket(Ticket ticket);
    }
}
