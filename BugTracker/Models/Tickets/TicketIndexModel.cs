using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Tickets
{
    public class TicketIndexModel
    {
        public IList<TicketListingModel> Tickets { get; set; } = new List<TicketListingModel>();
    }
}
