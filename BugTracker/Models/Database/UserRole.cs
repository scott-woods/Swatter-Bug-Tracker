using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Models.Database
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
