using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Tickets
{
    public class TicketIndexModel
    {
        public IList<TicketListingModel> Tickets { get; set; } = new List<TicketListingModel>();
        public Dictionary<Priority, int> PriorityCount { get; set; } = new Dictionary<Priority, int>();
        public Dictionary<Ticket_Type, int> TypeCount { get; set; } = new Dictionary<Ticket_Type, int>();
        public Dictionary<Status, int> StatusCount { get; set; } = new Dictionary<Status, int>();
    }
}
