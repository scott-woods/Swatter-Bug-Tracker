using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    interface IUserServices
    {
        public IEnumerable<ApplicationUser> GetAll();

        public void Add(ApplicationUser user);

        public ApplicationUser GetById(int id);
    }
}
