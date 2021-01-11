using BugTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Data
{
    //Contains all the User Tables
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.CreateDate)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Comments)
                .WithOne(t => t.Ticket)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Comment>()
                .Property(c => c.CreateDate)
                .HasDefaultValueSql("getdate()");
            
            modelBuilder.Entity<Project>()
                .Property(p => p.CreateDate)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tickets)
                .WithOne(p => p.Project)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectUsers)
                .WithOne(p => p.Project)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ProjectUsers)
                .WithOne(u => u.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ProjectUser>()
                .HasKey(bc => new { bc.UserId, bc.ProjectId });
            modelBuilder.Entity<ProjectUser>()
                .HasOne(bc => bc.ApplicationUser)
                .WithMany(b => b.ProjectUsers)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<ProjectUser>()
                .HasOne(bc => bc.Project)
                .WithMany(c => c.ProjectUsers)
                .HasForeignKey(bc => bc.ProjectId);
        }
    }
}
