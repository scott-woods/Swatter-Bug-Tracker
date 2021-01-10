using BugTracker.Models.PostModels;
using BugTracker.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.CommonViewModels
{
    public class TicketCommentModel
    {
        public TicketListingModel TicketModel { get; set; }
        public CommentModel CommentModel { get; set; }
    }
}
