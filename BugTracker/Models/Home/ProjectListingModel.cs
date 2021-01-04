using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Home
{
    public class ProjectListingModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatorId { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUpdatedById { get; set; }
        public string LastUpdatedByUsername { get; set; }
    }
}
