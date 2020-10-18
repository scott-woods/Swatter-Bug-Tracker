using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        public string Description { get; set; }

        public ApplicationUser AssignedUser { get; set; }

        public ApplicationUser Submitter { get; set; }

        public Project Project { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Category { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public ApplicationUser LastUpdatedBy { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
