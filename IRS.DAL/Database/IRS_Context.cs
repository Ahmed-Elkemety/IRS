using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IRS.DAL.Database
{
    public class IRS_Context : IdentityDbContext
    {
        public readonly object RefreshToken;
        public IRS_Context(DbContextOptions<IRS_Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IRS_Context).Assembly);



            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                    .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AdminChat> AdminChats { get; set; }
        public DbSet<Otp> otps { get; set; }
        public DbSet<PendingCitizenRegistration> pendingCitizenRegistrations { get; set; }
        public DbSet<pendingAuthorityRegistrations> pendingAuthorityRegistrations { get; set; }

    }
}
