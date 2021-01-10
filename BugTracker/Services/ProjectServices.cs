using BugTracker.Data;
using BugTracker.Models;
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

        public ProjectServices(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        public IEnumerable<Project> GetAll()
        {
            return _context.Projects
                .Include(p => p.Creator);
        }

        public Project GetById(int id)
        {
            return _context.Projects.Where(p => p.Id == id)
                .Include(p => p.Creator)
                .FirstOrDefault();
        }
    }
}
