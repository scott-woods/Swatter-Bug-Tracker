using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.PostModels
{
    public class ProjectModel
    {
        [Required(ErrorMessage = "Please provide a Title for the Project.")]
        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Please select at least 1 User.")]
        public IList<string> UserIds { get; set; }
    }
}
