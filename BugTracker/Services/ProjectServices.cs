﻿using BugTracker.Data;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class ProjectServices : IProjectServices
    {
        private AppDbContext _context;

        public ProjectServices(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Project project)
        {
            _context.Add(project);
            _context.SaveChanges();
        }

        public IEnumerable<Project> GetAll()
        {
            return _context.Projects;
        }

        public Project GetById(int id)
        {
            return _context.Projects.Find(id);
        }
    }
}
