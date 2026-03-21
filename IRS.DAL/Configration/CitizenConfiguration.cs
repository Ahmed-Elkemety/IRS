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
    public class CitizenConfiguration : IEntityTypeConfiguration<Citizen>
    {
        public void Configure(EntityTypeBuilder<Citizen> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.NationalId)
                .IsRequired()
                .HasMaxLength(20);

            // 1:1 مع User
            builder.HasOne(c => c.User)
                .WithOne(u => u.Citizen)
                .HasForeignKey<Citizen>(c => c.UserId);

            builder.HasIndex(c => c.UserId).IsUnique();
        }
    }
}
