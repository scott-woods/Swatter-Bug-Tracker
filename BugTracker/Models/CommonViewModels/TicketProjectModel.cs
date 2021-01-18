using BugTracker.Models.Projects;
using BugTracker.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.CommonViewModels
{
    public class TicketProjectModel
    {
        public TicketIndexModel TicketIndexModel { get; set; }
        public ProjectIndexModel ProjectIndexModel { get; set; }
    }
}
