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
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a Title for the Project.")]
        [StringLength(30)]
        public string Title { get; set; }

        public string Description { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public DateTime CreateDate { get; set; } = System.DateTime.Now;

        public ApplicationUser LastUpdatedBy { get; set; }

        public DateTime LastUpdateDate { get; set; } = System.DateTime.Now;

        public IList<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
