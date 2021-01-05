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
            _context.Add(user);
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.Users;
        }

        public ApplicationUser GetById(int id)
        {
            return _context.Users.Find(id);
        }
    }
}
