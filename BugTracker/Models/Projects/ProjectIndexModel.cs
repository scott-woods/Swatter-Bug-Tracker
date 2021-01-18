using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Projects
{
    public class ProjectIndexModel
    {
        public List<ProjectListingModel> Projects { get; set; } = new List<ProjectListingModel>();
    }
}
