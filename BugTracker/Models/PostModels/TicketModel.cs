using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.PostModels
{
    public class TicketModel
    {
        public int ProjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DeveloperId { get; set; }

        [EnumDataType(typeof(Status))]
        public Status TicketStatus { get; set; }

        [EnumDataType(typeof(Priority))]
        public Priority TicketPriority { get; set; }

        [EnumDataType(typeof(Ticket_Type))]
        public Ticket_Type TicketType { get; set; }
    }
}
