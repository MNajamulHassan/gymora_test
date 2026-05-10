using Microsoft.AspNetCore.Identity;

namespace Gymora.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CNIC { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? HealthConditions { get; set; }
        public string? FitnessGoals { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? JoinDate { get; set; }
        public Guid? CurrentPlanId { get; set; }
        public MembershipPlan? CurrentPlan { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
