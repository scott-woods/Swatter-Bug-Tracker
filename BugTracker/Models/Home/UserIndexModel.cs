using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Home
{
    public class UserIndexModel
    {
        public List<UserListingModel> Users { get; set; }
        
        public List<UserListingModel> GetAllDevelopers()
        {
            var v = from user in Users where user.Roles.Contains("Developer") select user;
            return v.ToList();
        }

        public List<UserListingModel> GetAllAdmins()
        {
            var v = from user in Users where user.Roles.Contains("Admin") select user;
            return v.ToList();
        }

        public List<UserListingModel> GetAllSubmitters()
        {
            var v = from user in Users where user.Roles.Contains("Submitter") select user;
            return v.ToList();
        }

        public List<UserListingModel> GetAllManagers()
        {
            var v = from user in Users where user.Roles.Contains("Manager") select user;
            return v.ToList();
        }
    }
}
