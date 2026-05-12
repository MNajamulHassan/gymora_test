using System.ComponentModel.DataAnnotations;

namespace Gymora.Data.Entities
{
    public class MembershipPlan
    {
        [Key]
        public Guid PlanId { get; set; } = Guid.NewGuid();

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        [Required]
        [StringLength(150)]
        public string PlanName { get; set; } = string.Empty;

        [Range(1, 3650)]
        public int DurationDays { get; set; }

        public Guid? AssignedTrainerId { get; set; }
        public Trainer? AssignedTrainer { get; set; }

        public int MaxMembers { get; set; } = 1;

        [Range(0, 999999)]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
