using BugTracker.Models;
using BugTracker.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public interface IUserServices
    {
        public IEnumerable<ApplicationUser> GetAll();

        public Task<IEnumerable<ApplicationUser>> GetAllByRole(string roleName, IEnumerable<ApplicationUser> users = default);

        public void Add(ApplicationUser user);

        public Task AddRole(ApplicationUser user, string roleName);

        public Task RemoveRole(ApplicationUser user, string roleName);

        public ApplicationUser GetById(int id);

        //public Task<IEnumerable<ApplicationUser>> GetAllByRole(string roleName);

        public Task<List<UserListingModel>> FormatUsersAsync(IEnumerable<ApplicationUser> userModels);
    }
}
