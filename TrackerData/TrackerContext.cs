using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using TrackerData.Models;

namespace TrackerData
{
    public class TrackerContext : DbContext
    {
        public TrackerContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
