using BugTracker.Models.Database;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public IList<ProjectUser> ProjectUsers { get; set; }
    }
}
