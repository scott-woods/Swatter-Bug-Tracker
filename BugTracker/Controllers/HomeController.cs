using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using BugTracker.Models.Email;
using BugTracker.Services;
using BugTracker.Models.CommonViewModels;
using System.Dynamic;
using Microsoft.EntityFrameworkCore.Internal;
using BugTracker.Models.PostModels;
using BugTracker.Data;
using BugTracker.Models.Projects;
using BugTracker.Models.Home;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IUserServices _userServices;
        private readonly IProjectServices _projectServices;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IUserServices userServices,
            IProjectServices projectServices,
            AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            _userServices = userServices;
            _projectServices = projectServices;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ManageRoles()
        {
            var userModels = _userServices.GetAll();

            var listingResult = await FormatUsersAsync(userModels);

            var userModel = new UserIndexModel()
            {
                Users = listingResult
            };

            var roleModel = new AlterRoleModel();

            var model = new UserRoleModel()
            {
                UserModel = userModel,
                RoleModel = roleModel
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ManageRoles(UserRoleModel model, string addRoles, string removeRoles)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            foreach (var id in model.RoleModel.UserIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (!string.IsNullOrEmpty(addRoles))
                {
                    if (model.RoleModel.SubmitterChecked) await _userManager.AddToRoleAsync(user, "Submitter");
                    if (model.RoleModel.DeveloperChecked) await _userManager.AddToRoleAsync(user, "Developer");
                    if (model.RoleModel.ManagerChecked) await _userManager.AddToRoleAsync(user, "Manager");
                    if (model.RoleModel.AdminChecked) await _userManager.AddToRoleAsync(user, "Admin");
                }
                if (!string.IsNullOrEmpty(removeRoles))
                {
                    if (model.RoleModel.SubmitterChecked) await _userManager.RemoveFromRoleAsync(user, "Submitter");
                    if (model.RoleModel.DeveloperChecked) await _userManager.RemoveFromRoleAsync(user, "Developer");
                    if (model.RoleModel.ManagerChecked) await _userManager.RemoveFromRoleAsync(user, "Manager");
                    if (model.RoleModel.AdminChecked) await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                //Get Index of matching User in UserModel
                int index = model.UserModel.Users.IndexOf(model.UserModel.Users.Where(u => u.Id == id).FirstOrDefault());
                IList<string> roles = await _userManager.GetRolesAsync(user);
                string joined = string.Join(", ", roles);
                model.UserModel.Users[index].Roles = joined;
            }

            ViewBag.ChangeConfirmation = "Changes have been saved.";
            model.RoleModel.Reset();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"User {User.Identity.Name} has logged out.");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Helper Functions
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
        public async Task<ProjectListingModel> FormatProjectAsync(Project project)
        {
            //Format project into Project Listing Model
            var listingResult = new ProjectListingModel
            {
                EfProject = project,
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreateDate = project.CreateDate,
                Creator = (project.Creator == null ? null : project.Creator),
                LastUpdateDate = project.LastUpdateDate,
                LastUpdatedById = (project.LastUpdatedBy == null ? "N/A" : project.LastUpdatedBy.Id),
                LastUpdatedByUsername = (project.LastUpdatedBy == null ? "N/A" : project.LastUpdatedBy.UserName)
            };

            //Add a UserIndexModel of associated ApplicationUsers to Listing Model
            var appUsers = new List<ApplicationUser>();
            var projectUsers = _context.ProjectUsers.Where(p => p.ProjectId == project.Id).ToList();
            for (int i = 0; i < projectUsers.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(projectUsers[i].UserId);
                appUsers.Add(user);
            }
            listingResult.ProjectUsers.Users = await FormatUsersAsync(appUsers);

            //Add list of associated Tickets to Listing Model
            var projectTickets = _context.Tickets.Where(t => t.Project.Id == project.Id).ToList();
            for (int i = 0; i < projectTickets.Count; i++)
            {
                listingResult.Tickets.Add(projectTickets[i]);
            }

            return listingResult;
        }
    }
}
