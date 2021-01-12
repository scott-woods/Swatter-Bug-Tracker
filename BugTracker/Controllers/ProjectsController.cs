using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.CommonViewModels;
using BugTracker.Models.Email;
using BugTracker.Models.Home;
using BugTracker.Models.PostModels;
using BugTracker.Models.Projects;
using BugTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IUserServices _userServices;
        private readonly IProjectServices _projectServices;
        private readonly ITicketServices _ticketServices;
        private readonly AppDbContext _context;

        public ProjectsController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IUserServices userServices,
            IProjectServices projectServices,
            ITicketServices ticketServices,
            AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            _userServices = userServices;
            _projectServices = projectServices;
            _ticketServices = ticketServices;
            _context = context;
        }
        
        public IActionResult Index()
        {
            var allProjects = _projectServices.GetAll();

            var listingResult = allProjects
                .Select(result => new ProjectListingModel
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Description,
                    CreateDate = result.CreateDate,
                    Creator = result.Creator,
                    LastUpdateDate = result.LastUpdateDate,
                    LastUpdatedBy = result.LastUpdatedBy
                }
                    ).ToList();

            var projectModel = new ProjectIndexModel
            {
                Projects = listingResult
            };

            return View(projectModel);
        }

        [HttpGet]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> NewProject()
        {
            //Get all Users from DB and format into List of UserListingModels
            var allUsers = _userServices.GetAll();
            var userModels = await _userServices.FormatUsersAsync(allUsers);

            //Create UserIndexModel from List
            var userIndex = new UserIndexModel
            {
                Users = userModels
            };

            //Create Common Model for Project and Users
            var projectModel = new ProjectModel();
            var model = new UserProjectModel
            {
                UserModel = userIndex,
                ProjectModel = projectModel
            };

            ViewData["cardHeader"] = "Create New Project";
            ViewData["buttonLabel"] = "Create Project";
            ViewData["formTitle"] = "newProjectForm";
            ViewData["aspAction"] = "NewProject";

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> NewProject(UserProjectModel model, string[] userIds)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newProject = new Project
            {
                Title = model.ProjectModel.Title,
                Description = model.ProjectModel.Description,
                CreateDate = DateTime.Now,
                Creator = await _userManager.GetUserAsync(User),
                LastUpdatedBy = await _userManager.GetUserAsync(User),
                LastUpdateDate = DateTime.Now
            };

            List<ApplicationUser> selectedUsers = new List<ApplicationUser>();

            foreach (var id in userIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                selectedUsers.Add(user);
            }

            for (int i = 0; i < selectedUsers.Count; i++)
            {
                var projUser = new ProjectUser
                {
                    ApplicationUser = selectedUsers[i],
                    Project = newProject
                };
                newProject.ProjectUsers.Add(projUser);
            }

            _context.Projects.Add(newProject);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ProjectDetails(int projectId)
        {
            //Get project from DB
            var project = _projectServices.GetById(projectId);

            //Format Project into Listing Model (adds list of Users and Tickets)
            var listingResult = await _projectServices.FormatProjectAsync(project);

            foreach(var ticket in listingResult.Tickets)
            {
                _logger.LogInformation("Ticket Title: " + ticket.Title);
            }

            return View(listingResult);
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(int projectId)
        {
            var project = _projectServices.GetById(projectId);
            var listingResult = await _projectServices.FormatProjectAsync(project);

            //Add IDs of all Users in project to list
            List<string> userIds = new List<string>();
            foreach (var user in listingResult.ProjectUsers.Users)
            {
                userIds.Add(user.Id);
            }

            var projectModel = new ProjectModel
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                UserIds = userIds
            };

            var userModel = new UserIndexModel
            {
                Users = await _userServices.FormatUsersAsync(_userServices.GetAll())
            };

            var model = new UserProjectModel
            {
                UserModel = userModel,
                ProjectModel = projectModel
            };

            ViewData["cardHeader"] = "Edit Project";
            ViewData["buttonLabel"] = "Save Changes";
            ViewData["formTitle"] = "editProjectForm";
            ViewData["aspAction"] = "EditProject";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(UserProjectModel model, string[] userIds)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Get Project from DB and change appropriate fields
            var project = _projectServices.GetById(model.ProjectModel.Id);
            project.Title = model.ProjectModel.Title;
            project.Description = model.ProjectModel.Description;
            project.LastUpdatedBy = await _userManager.GetUserAsync(User);
            project.LastUpdateDate = DateTime.Now;

            //Get List of ProjectUsers and their IDs that were already associated with the project
            var projectUsers = _context.ProjectUsers.Where(p => p.ProjectId == project.Id).ToList();
            var projUserIds = new List<string>();
            foreach (var user in projectUsers)
            {
                projUserIds.Add(user.UserId);

                //Remove existing User from ProjectUsers if they were not selected
                if (!userIds.Contains(user.UserId))
                {
                    _context.Remove(user);
                }
            }

            //Add selected Users to Project
            List<ApplicationUser> selectedUsers = new List<ApplicationUser>();
            foreach (var id in userIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                selectedUsers.Add(user);
            }
            for (int i = 0; i < selectedUsers.Count; i++)
            {
                var projUser = new ProjectUser
                {
                    UserId = selectedUsers[i].Id,
                    ApplicationUser = selectedUsers[i],
                    Project = project,
                    ProjectId = project.Id
                };
                //If project didn't already have this User, add them
                if (!projUserIds.Contains(projUser.UserId))
                {
                    project.ProjectUsers.Add(projUser);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("ProjectDetails", new { projectId = project.Id });
        }

        [HttpPost]
        public IActionResult DeleteProject(int id)
        {
            Project project = _projectServices.GetById(id);
            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
