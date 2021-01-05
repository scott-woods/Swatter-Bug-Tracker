using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class ProjectUser
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
