using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class AlterRoleModel
    {
        [Required(ErrorMessage = "Please select at least 1 User.")]
        public IEnumerable<string> UserIds { get; set; }

        public bool AdminChecked { get; set; }
        public bool ManagerChecked { get; set; }
        public bool DeveloperChecked { get; set; }
        public bool SubmitterChecked { get; set; }

        public void Reset()
        {
            UserIds = null;
            AdminChecked = false;
            ManagerChecked = false;
            DeveloperChecked = false;
            SubmitterChecked = false;
        }
    }
}
