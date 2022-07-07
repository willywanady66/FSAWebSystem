using FSAWebSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;

namespace FSAWebSystem.Models.Context
{
	public class FSAWebSystemDbContext : IdentityDbContext<FSAWebSystemUser>
    {
        public FSAWebSystemDbContext(DbContextOptions<FSAWebSystemDbContext> options)
       : base(options)
        {
            Database.SetCommandTimeout(900);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<SKU>()
                .Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Entity<Banner>()
                .Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Entity<UserUnilever>()
              .Property(x => x.IsActive)
              .HasDefaultValue(true); 
            
            builder.Entity<WorkLevel>()
              .Property(x => x.IsActive)
              .HasDefaultValue(true);
        }

        public DbSet<UserUnilever> UsersUnilever { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<SKU> SKUs { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<RoleUnilever> RoleUnilevers { get; set; }
        public DbSet<RoleAccess> RoleAccesses { get; set; }
        public DbSet<FSACalendarHeader> FSACalendarHeader { get; set; }
        public DbSet<FSACalendarDetail> FSACalendarDetail { get; set; }
        public DbSet<FSADocument> FSADocuments{ get; set; }
        public DbSet<MonthlyBucket> MonthlyBuckets{ get; set; }
        public DbSet<WeeklyBucket> WeeklyBuckets{ get; set; }
        public DbSet<WorkLevel> WorkLevels{ get; set; }
        public DbSet<Proposal> Proposals { get; set; }
    }
}
