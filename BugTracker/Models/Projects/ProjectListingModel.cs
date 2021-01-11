using BugTracker.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Projects
{
    public class ProjectListingModel
    {
        public Project EfProject { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public ApplicationUser Creator { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public ApplicationUser LastUpdatedBy { get; set; }
        public UserIndexModel ProjectUsers { get; set; } = new UserIndexModel();
        public UserIndexModel AllUsers { get; set; } = new UserIndexModel();
        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
