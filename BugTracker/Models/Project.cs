using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        public string Description { get; set; }

        public ApplicationUser Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public ApplicationUser LastUpdatedBy { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public IList<ProjectUsers> ProjectUsers { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
