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
    public class PendingCitizenRegistrationConfiguration : IEntityTypeConfiguration<PendingCitizenRegistration>
    {
        public void Configure(EntityTypeBuilder<PendingCitizenRegistration> builder)
        {
            // Table Name
            builder.ToTable("PendingCitizenRegistrations");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Password)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.NationalId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(p => p.Email).IsUnique();
        }
    }
}
