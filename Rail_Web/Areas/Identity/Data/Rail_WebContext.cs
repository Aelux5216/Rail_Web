using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rail_Web.Areas.Identity.Data;
using Rail_Web.Areas.Identity.Pages.Account.Manage;

namespace Rail_Web.Models
{
    public class Rail_WebContext : IdentityDbContext<Rail_WebUser>
    {
        public Rail_WebContext(DbContextOptions<Rail_WebContext> options)
            : base(options)
        {

        }

        public Rail_WebContext()
        {

        }

        public virtual DbSet<OrderHistoryModel.OrderHistory> OrderHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrderHistoryModel.OrderHistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("OrderHistory");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Username)
                    .HasColumnName("Username")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.TicketRef)
                    .HasColumnName("TicketRef")
                    .HasColumnType("varchar(9)");

                entity.Property(e => e.TicketDet)
                    .HasColumnName("TicketDet")
                    .HasColumnType("varchar(500)");
            });

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
