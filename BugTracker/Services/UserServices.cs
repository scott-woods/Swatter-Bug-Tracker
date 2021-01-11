using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Home;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class UserServices : IUserServices
    {
        private AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public UserServices(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Add(ApplicationUser user)
        {
            _context.Users.Add(user);
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.Users;
        }

        public ApplicationUser GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public async Task<List<UserListingModel>> FormatUsersAsync(IEnumerable<ApplicationUser> userModels)
        {
            var listingResult = userModels
                .Select(result => new UserListingModel
                {
                    Id = result.Id,
                    Email = result.Email ?? "N/A",
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    UserName = result.UserName,
                    FullName = result.FirstName + " " + result.LastName
                }
                    ).ToList();

            for (int i = 0; i < listingResult.Count; i++)
            {
                //Adds each User's respective roles to the Listing
                var appUser = await _userManager.FindByIdAsync(listingResult[i].Id);
                var roles = _userManager.GetRolesAsync(appUser);
                IList<string> rolesResult = await roles;
                string joined = string.Join(", ", rolesResult);
                listingResult[i].Roles = joined;
            }
            return listingResult;
        }
    }
}
