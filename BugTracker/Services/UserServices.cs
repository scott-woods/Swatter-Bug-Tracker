using BugTracker.Controllers;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Database;
using BugTracker.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class UserServices : IUserServices
    {
        private ILogger<HomeController> _logger;
        private AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserServices(ILogger<HomeController> logger, AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Add(ApplicationUser user)
        {
            _context.Users.Add(user);
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.Users;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllByRole(string roleName, IEnumerable<ApplicationUser> users = default)
        {
            var allDevelopers = await _userManager.GetUsersInRoleAsync(roleName);
            if (users == default(IEnumerable<ApplicationUser>))
            {
                return allDevelopers;
            }
            return users.Where(u => allDevelopers.Any(d => d.Id == u.Id));
        }

        public async Task AddRole(ApplicationUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task RemoveRole(ApplicationUser user, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        //public async Task<IEnumerable<ApplicationUser>> GetAllByRole(string roleName)
        //{
        //    var role = await _roleManager.FindByNameAsync(roleName);
        //    var id = await _roleManager.GetRoleIdAsync(role);
        //    var allUsers = GetAll();
        //    foreach (var user in allUsers)
        //    {
        //        foreach (var userRole in user.UserRoles)
        //        {
        //            _logger.LogInformation("User ID: " + userRole.UserId);
        //            _logger.LogInformation("Role ID: " + userRole.RoleId);
        //        }
        //    }
        //    return _context.Users.Where(u => u.UserRoles.Any(r => r.RoleId == id));
        //}

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
