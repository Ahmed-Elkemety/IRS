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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Citizen
            builder.HasOne(n => n.Citizen)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CitizenId);

            // Report
            builder.HasOne(n => n.Report)
                .WithMany(r => r.Notifications)
                .HasForeignKey(n => n.ReportId);
        }
    }
}
