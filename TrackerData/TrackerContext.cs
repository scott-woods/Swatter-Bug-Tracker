using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using TrackerData.Models;

namespace TrackerData
{
    public class TrackerContext : IdentityDbContext
    {
        public TrackerContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
