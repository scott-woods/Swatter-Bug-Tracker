using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        public string Description { get; set; }

        public ApplicationUser AssignedDeveloper { get; set; }

        public ApplicationUser Creator { get; set; }

        public Project Project { get; set; }

        public Priority TicketPriority { get; set; }

        public Status TicketStatus { get; set; }

        public Ticket_Type TicketType { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public ApplicationUser LastUpdatedBy { get; set; }

        public IList<Comment> Comments { get; set; }
    }

    public enum Status
    {
        Open,

        [Display(Name = "In Progress")]
        In_Progress,

        Resolved
    }
    public enum Priority
    {
        Low,

        Medium,

        High
    }
    public enum Ticket_Type
    {
        [Display(Name = "Bugs/Errors")]
        Bugs,

        [Display(Name = "Feature Requests")]
        Feature_Requests,

        Other
    }
}
