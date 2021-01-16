using BugTracker.Models;
using BugTracker.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public interface IProjectServices
    {
        public IEnumerable<Project> GetAll();

        public IEnumerable<Project> GetAllByUserId(string id);
        
        public void Add(Project project);

        public Project GetById(int id);

        public Task<ProjectListingModel> FormatProjectAsync(Project project);
    }
}
