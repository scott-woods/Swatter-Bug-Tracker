using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
    public class ApplicationRole : IdentityRole<string>
    {
        public IList<UserRole> UserRoles { get; set; }
    }
}
