using FSAWebSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.ApprovalSystemCheckModel;

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

            builder.Entity<BannerPlant>()
                .Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Entity<UserUnilever>()
              .Property(x => x.IsActive)
              .HasDefaultValue(true); 
            
            builder.Entity<WorkLevel>()
              .Property(x => x.IsActive)
              .HasDefaultValue(true);

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

        }

        public DbSet<UserUnilever> UsersUnilever { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<BannerPlant> BannerPlants { get; set; }
        public DbSet<SKU> SKUs { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<RoleUnilever> RoleUnilevers { get; set; }
        public DbSet<FSACalendarHeader> FSACalendarHeaders { get; set; }
        public DbSet<FSACalendarDetail> FSACalendarDetails { get; set; }
        public DbSet<ULICalendar> ULICalendars { get; set; }
        public DbSet<ULICalendarDetail> ULICalendarDetails { get; set; }
        public DbSet<FSADocument> FSADocuments{ get; set; }
        public DbSet<MonthlyBucket> MonthlyBuckets{ get; set; }
        public DbSet<WeeklyBucket> WeeklyBuckets{ get; set; }
        public DbSet<WorkLevel> WorkLevels{ get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalDetail> ProposalDetails { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<WeeklyBucketHistory> WeeklyBucketHistories { get; set; }
        public DbSet<ProposalHistory> ProposalHistories { get; set; }
        public DbSet<AndromedaModel> Andromedas { get; set; }
        public DbSet<BottomPriceModel> BottomPrices { get; set; }
        public DbSet<ITrustModel> ITrusts{ get; set; }
    }
}
