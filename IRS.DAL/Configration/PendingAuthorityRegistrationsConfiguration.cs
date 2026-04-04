using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRS.DAL.Configration
{
    public class PendingAuthorityRegistrationsConfiguration : IEntityTypeConfiguration<pendingAuthorityRegistrations>
    {
        public void Configure(EntityTypeBuilder<pendingAuthorityRegistrations> builder)
        {
            // Table Name
            builder.ToTable("PendingAuthorityRegistrations");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Email
            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(255);

            // Password
            builder.Property(p => p.Password)
                .IsRequired()
                .HasMaxLength(500);

            // Name
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Phone
            builder.Property(p => p.Phone)
                .IsRequired()
                .HasMaxLength(20);

            // Address
            builder.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(300);

            // DeptId
            builder.Property(p => p.DeptId)
                .IsRequired();

            // CreatedAt
            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Unique Email
            builder.HasIndex(p => p.Email).IsUnique();
        }
    }
}
