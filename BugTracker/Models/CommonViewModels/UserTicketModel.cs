using BugTracker.Models.Home;
using BugTracker.Models.PostModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.CommonViewModels
{
    public class UserTicketModel
    {
        public TicketModel TicketModel { get; set; }

        public UserIndexModel UserIndexModel { get; set; }
    }
}
