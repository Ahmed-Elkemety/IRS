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
    public class AdminChatConfiguration : IEntityTypeConfiguration<AdminChat>
    {
        public void Configure(EntityTypeBuilder<AdminChat> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(a => a.Citizen)
                .WithMany(c => c.AdminChats)
                .HasForeignKey(a => a.CitizenId);
        }
    }
}
