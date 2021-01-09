using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.PostModels
{
    public class ProjectModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a Title for the Project.")]
        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public IList<string> UserIds { get; set; } = new List<string>();
    }
}
