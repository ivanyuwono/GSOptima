using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GSOptima.Models;

namespace GSOptima.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<StockPrice>()
            .HasKey(c => new { c.StockID, c.Date });

            builder.Entity<StockWatchList>()
           .HasKey(t => new { t.ApplicationUserId, t.StockID });
        }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockPrice> StockPrice { get; set; }
        public DbSet<StockWatchList> StockWatchList { get; set; }

        public DbSet<GSProWatchList> GSProWatchList { get; set; }
    }
}
