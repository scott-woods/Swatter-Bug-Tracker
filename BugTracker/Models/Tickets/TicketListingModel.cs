using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Tickets
{
    public class TicketListingModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Project Project { get; set; }
        public ApplicationUser AssignedDeveloper { get; set; }
        public DateTime CreateDate { get; set; }
        public ApplicationUser Creator { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public ApplicationUser LastUpdatedBy { get; set; }
        public Priority TicketPriority { get; set; }
        public Status TicketStatus { get; set; }
        public Ticket_Type TicketType { get; set; }
        public IList<Comment> Comments { get; set; }
    }
}
