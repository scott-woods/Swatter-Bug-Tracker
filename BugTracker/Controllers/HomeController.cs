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
using BugTracker.Models.Home;
using System.Dynamic;
using Microsoft.EntityFrameworkCore.Internal;

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

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IUserServices userServices,
            IProjectServices projectServices)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            _userServices = userServices;
            _projectServices = projectServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Profile(ApplicationUser user)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Projects()
        {
            var allProjects = _projectServices.GetAll();

            var listingResult = allProjects
                .Select(result => new ProjectListingModel
                {
                    Id = result.Id.ToString(),
                    Title = result.Title,
                    Description = result.Description,
                    CreateDate = result.CreateDate,
                    CreatorId = (result.Creator == null ? "N/A" : result.Creator.Id),
                    CreatorUsername = (result.Creator == null ? "N/A" : result.Creator.UserName),
                    LastUpdateDate = result.LastUpdateDate,
                    LastUpdatedById = (result.LastUpdatedBy == null ? "N/A" : result.LastUpdatedBy.Id),
                    LastUpdatedByUsername = (result.LastUpdatedBy == null ? "N/A" : result.LastUpdatedBy.UserName)
                }
                    ).ToList();

            var projectModel = new ProjectIndexModel
            {
                Projects = listingResult
            };

            return View(projectModel);
        }

        public IActionResult ProjectDetails(string projectId)
        {
            var project = _projectServices.GetById(Int32.Parse(projectId));
            var listingResult = new ProjectListingModel
            {
                Id = project.Id.ToString(),
                Title = project.Title,
                Description = project.Description,
                CreateDate = project.CreateDate,
                CreatorId = (project.Creator == null ? "N/A" : project.Creator.Id),
                CreatorUsername = (project.Creator == null ? "N/A" : project.Creator.UserName),
                LastUpdateDate = project.LastUpdateDate,
                LastUpdatedById = (project.LastUpdatedBy == null ? "N/A" : project.LastUpdatedBy.Id),
                LastUpdatedByUsername = (project.LastUpdatedBy == null ? "N/A" : project.LastUpdatedBy.UserName)
            };

            return View(listingResult);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ManageRoles()
        {
            var userModels = _userServices.GetAll();
            
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
            
            for (int i=0; i < listingResult.Count(); i++)
            {
                //Adds each User's respective roles to the Listing
                var appUser = await _userManager.FindByIdAsync(listingResult[i].Id);
                var roles = _userManager.GetRolesAsync(appUser);
                IList<string> rolesResult = await roles;
                string joined = string.Join(", ", rolesResult);
                listingResult[i].Roles = joined;
            }
            
            var userModel = new UserIndexModel()
            {
                Users = listingResult
            };

            var roleModel = new AlterRoleModel();

            var model = new CommonViewModel()
            {
                UserModel = userModel,
                RoleModel = roleModel
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ManageRoles(CommonViewModel model, string addRoles, string removeRoles)
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
    }
}
