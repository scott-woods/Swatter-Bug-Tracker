using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.CommonViewModels;
using BugTracker.Models.Database;
//using BugTracker.Models.Email;
using BugTracker.Models.Home;
using BugTracker.Models.PostModels;
using BugTracker.Models.Projects;
using BugTracker.Models.Tickets;
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
        //private readonly IEmailSender _emailSender;
        private readonly IUserServices _userServices;
        private readonly IProjectServices _projectServices;
        private readonly ITicketServices _ticketServices;
        private readonly AppDbContext _context;

        public TicketsController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            //IEmailSender emailSender,
            IUserServices userServices,
            IProjectServices projectServices,
            ITicketServices ticketServices,
            AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            //_emailSender = emailSender;
            _userServices = userServices;
            _projectServices = projectServices;
            _ticketServices = ticketServices;
            _context = context;
        }

        public IActionResult Index()
        {
            var tickets = new List<Ticket>();
            //If User is Admin, show all Tickets
            if (User.IsInRole("Admin"))
            {
                tickets = _ticketServices.GetAll().ToList();
            }
            //Otherwise, show tickets only relative to the User.
            else
            {
                tickets = _ticketServices.GetAllByUser(_userManager.GetUserId(User)).ToList();
            }
            var model = new TicketIndexModel();
            foreach (var ticket in tickets)
            {
                model.Tickets.Add(_ticketServices.FormatTicket(ticket));
            }

            return View(model);
        }

        public IActionResult TicketDetails(int ticketId)
        {
            var ticket = _ticketServices.GetById(ticketId);
            var ticketModel = _ticketServices.FormatTicket(ticket);
            var model = new TicketCommentModel
            {
                TicketModel = ticketModel,
                CommentModel = new CommentModel()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(TicketCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("TicketDetails", new { ticketId = model.TicketModel.Id });
            }

            var ticket = _ticketServices.GetById(model.TicketModel.Id);
            var currentUser = await _userManager.GetUserAsync(User);
            var newComment = new Comment
            {
                Message = model.CommentModel.Message,
                Commenter = currentUser,
                CreateDate = DateTime.Now,
                Ticket = ticket
            };

            _context.Comments.Add(newComment);
            _context.SaveChanges();

            return RedirectToAction("TicketDetails", new { ticketId = model.TicketModel.Id });
        }

        [HttpGet]
        public async Task<IActionResult> NewTicket(int projectId)
        {
            //Get all Users and format into Index Model
            var project = _projectServices.GetById(projectId);
            var projUsers = _projectServices.GetAllUsers(project);
            var projDevelopers = await _userServices.GetAllByRole("Developer", projUsers);

            var formattedUsers = await _userServices.FormatUsersAsync(projDevelopers);
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticketModel = model.TicketModel;
            var currentUser = await _userManager.GetUserAsync(User);

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

            _ticketServices.Add(ticket);

            return RedirectToAction("ProjectDetails", "Projects", new { projectId = ticketModel.ProjectId });
        }

        [HttpGet]
        public async Task<IActionResult> EditTicket(int ticketId)
        {
            var allUsers = _userServices.GetAll();
            var formattedUsers = await _userServices.FormatUsersAsync(allUsers);
            var userIndex = new UserIndexModel { Users = formattedUsers };

            var ticket = _ticketServices.GetById(ticketId);
            var ticketModel = new TicketModel
            {
                Id = ticket.Id,
                ProjectId = ticket.Project.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                DeveloperId = ticket.AssignedDeveloper.Id,
                TicketPriority = ticket.TicketPriority,
                TicketStatus = ticket.TicketStatus,
                TicketType = ticket.TicketType
            };

            var model = new UserTicketModel
            {
                UserIndexModel = userIndex,
                TicketModel = ticketModel
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTicket(UserTicketModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticketModel = model.TicketModel;
            var currentUser = await _userManager.GetUserAsync(User);

            var ticket = _ticketServices.GetById(ticketModel.Id);
            ticket.Title = ticketModel.Title;
            ticket.Description = ticketModel.Description;
            ticket.AssignedDeveloper = await _userManager.FindByIdAsync(ticketModel.DeveloperId);
            ticket.LastUpdateDate = DateTime.Now;
            ticket.LastUpdatedBy = currentUser;
            ticket.TicketPriority = ticketModel.TicketPriority;
            ticket.TicketStatus = ticketModel.TicketStatus;
            ticket.TicketType = ticketModel.TicketType;

            _context.SaveChanges();

            return RedirectToAction("ProjectDetails", "Projects", new { projectId = ticketModel.ProjectId });
        }

        [HttpGet]
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _ticketServices.GetById(id);
            return PartialView("_DeleteTicketPartial", ticket);
        }

        [HttpPost]
        public IActionResult DeleteTicket(Ticket ticket)
        {
            var projId = ticket.Project.Id;
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return PartialView("_DeleteTicketPartial", ticket);
        }
    }
}
