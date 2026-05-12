using Gymora.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gymora.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
        public DbSet<Trainer> Trainers => Set<Trainer>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tenant>(e =>
            {
                e.HasKey(t => t.TenantId);
                e.HasIndex(t => t.Subdomain).IsUnique();
                e.Property(t => t.GymName).HasMaxLength(200).IsRequired();
                e.Property(t => t.Subdomain).HasMaxLength(100).IsRequired();
            });

            builder.Entity<ApplicationUser>(e =>
            {
                e.Property(u => u.FullName).HasMaxLength(200).IsRequired();
                e.Property(u => u.Gender).HasMaxLength(10);
                e.Property(u => u.CNIC).HasMaxLength(15);
                e.Property(u => u.Address).HasMaxLength(500);
                e.Property(u => u.EmergencyContact).HasMaxLength(200);
                e.Property(u => u.ProfilePhotoUrl).HasMaxLength(500);
                e.Property(u => u.HealthConditions).HasMaxLength(1000);
                e.Property(u => u.FitnessGoals).HasMaxLength(500);

                e.HasOne(u => u.Tenant)
                 .WithMany(t => t.Users)
                 .HasForeignKey(u => u.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(u => u.CurrentPlan)
                 .WithMany()
                 .HasForeignKey(u => u.CurrentPlanId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<MembershipPlan>(e =>
            {
                e.HasKey(p => p.PlanId);
                e.Property(p => p.PlanName).HasMaxLength(150).IsRequired();
                e.Property(p => p.Description).HasMaxLength(1000);
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");

                e.HasOne(p => p.Tenant)
                 .WithMany()
                 .HasForeignKey(p => p.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(p => new { p.TenantId, p.PlanName });
                e.HasOne(p => p.AssignedTrainer)
                 .WithMany()
                 .HasForeignKey(p => p.AssignedTrainerId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Trainer>(e =>
            {
                e.HasKey(t => t.TrainerId);
                e.Property(t => t.FullName).HasMaxLength(200).IsRequired();
                e.Property(t => t.Specialization).HasMaxLength(200);
                e.Property(t => t.PhoneNumber).HasMaxLength(30);
                e.HasIndex(t => new { t.TenantId, t.FullName });
            });

            // Seed demo gym tenant
            var demoTenantId = new Guid("11111111-1111-1111-1111-111111111111");
            builder.Entity<Tenant>().HasData(new Tenant
            {
                TenantId = demoTenantId,
                GymName = "Iron Peak Gym",
                Subdomain = "ironpeak",
                Status = "Active",
                GracePeriodDays = 3,
                CreatedAt = new DateTime(2025, 1, 1)
            });
        }
    }
}
