using BugTracker.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.CommonViewModels
{
    public class UserRoleModel
    {
        public UserIndexModel UserModel { get; set; }
        
        public AlterRoleModel RoleModel { get; set; }
    }
}
