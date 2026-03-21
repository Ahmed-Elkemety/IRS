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
    public class AuthorityConfiguration : IEntityTypeConfiguration<Authority>
    {
        public void Configure(EntityTypeBuilder<Authority> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 1:1 مع User
            builder.HasOne(a => a.User)
                .WithOne(u => u.Authority)
                .HasForeignKey<Authority>(a => a.UserId);

            builder.HasIndex(a => a.UserId).IsUnique();

            // Department
            builder.HasOne(a => a.Department)
                .WithMany(d => d.Authorities)
                .HasForeignKey(a => a.DeptId);
        }
    }
}
