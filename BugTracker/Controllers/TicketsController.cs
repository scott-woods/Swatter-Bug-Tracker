using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.CommonViewModels;
using BugTracker.Models.Email;
using BugTracker.Models.Home;
using BugTracker.Models.PostModels;
using BugTracker.Models.Projects;
using BugTracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IUserServices _userServices;
        private readonly IProjectServices _projectServices;
        private readonly AppDbContext _context;

        public TicketsController(ILogger<HomeController> logger,
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
        public async Task<IActionResult> NewTicket(int projectId)
        {
            //Get all Users and format into Index Model
            var allUsers = _userServices.GetAll();
            var formattedUsers = await FormatUsersAsync(allUsers);
            var userIndex = new UserIndexModel { Users = formattedUsers };

            var ticketModel = new TicketModel { ProjectId = projectId };

            var model = new UserTicketModel
            {
                UserIndexModel = userIndex,
                TicketModel = ticketModel
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewTicket(UserTicketModel model)
        {
            var ticketModel = model.TicketModel;
            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation(ticketModel.Title);
            _logger.LogInformation(ticketModel.Description);
            _logger.LogInformation(ticketModel.ProjectId.ToString());
            _logger.LogInformation(ticketModel.DeveloperId);
            _logger.LogInformation(ticketModel.TicketPriority.ToString());
            _logger.LogInformation(ticketModel.TicketStatus.ToString());
            _logger.LogInformation(ticketModel.TicketType.ToString());
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticket = new Ticket
            {
                Title = ticketModel.Title,
                Description = ticketModel.Description,
                Project = _projectServices.GetById(ticketModel.ProjectId),
                AssignedDeveloper = await _userManager.FindByIdAsync(ticketModel.DeveloperId),
                CreateDate = DateTime.Now,
                LastUpdateDate = DateTime.Now,
                LastUpdatedBy = currentUser,
                Creator = currentUser,
                TicketPriority = ticketModel.TicketPriority,
                TicketStatus = ticketModel.TicketStatus,
                TicketType = ticketModel.TicketType
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("ProjectDetails", "Projects", new { projectId = model.TicketModel.ProjectId });
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
