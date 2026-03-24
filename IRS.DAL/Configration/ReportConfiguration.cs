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
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Description)
                .IsRequired();

            builder.Property(r => r.Location)
                .HasColumnType("geography");

            builder.Property(r => r.Image)
               .HasColumnType("varbinary(max)")
               .IsRequired(false);

            // Citizen
            builder.HasOne(r => r.Citizen)
                .WithMany(c => c.Reports)
                .HasForeignKey(r => r.CitizenId);

            // Authority
            builder.HasOne(r => r.Authority)
                .WithMany(a => a.Reports)
                .HasForeignKey(r => r.AuthorityId);

            // Category
            builder.HasOne(r => r.Category)
                .WithMany(c => c.Reports)
                .HasForeignKey(r => r.CategoryId);
        }
    }
}
