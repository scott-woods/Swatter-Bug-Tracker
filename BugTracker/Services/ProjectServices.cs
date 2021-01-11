using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Projects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketServices _ticketServices;
        private readonly IUserServices _userServices;

        public ProjectServices(AppDbContext context, 
            UserManager<ApplicationUser> userManager,
            ITicketServices ticketServices,
            IUserServices userServices)
        {
            _context = context;
            _userManager = userManager;
            _ticketServices = ticketServices;
            _userServices = userServices;
        }

        public void Add(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        public IEnumerable<Project> GetAll()
        {
            return _context.Projects
                .Include(p => p.LastUpdatedBy)
                .Include(p => p.Creator);
        }

        public Project GetById(int id)
        {
            return _context.Projects.Where(p => p.Id == id)
                .Include(p => p.LastUpdatedBy)
                .Include(p => p.Creator)
                .FirstOrDefault();
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
                Creator = project.Creator,
                LastUpdateDate = project.LastUpdateDate,
                LastUpdatedBy = project.LastUpdatedBy
            };

            //Add a UserIndexModel of associated ApplicationUsers to Listing Model
            var appUsers = new List<ApplicationUser>();
            var projectUsers = _context.ProjectUsers.Where(p => p.ProjectId == project.Id).ToList();
            for (int i = 0; i < projectUsers.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(projectUsers[i].UserId);
                appUsers.Add(user);
            }
            listingResult.ProjectUsers.Users = await _userServices.FormatUsersAsync(appUsers);

            //Add list of associated Tickets to Listing Model
            var projectTickets = _ticketServices.GetAllByProjectId(project.Id).ToList();
            for (int i = 0; i < projectTickets.Count; i++)
            {
                listingResult.Tickets.Add(projectTickets[i]);
            }

            return listingResult;
        }
    }
}
