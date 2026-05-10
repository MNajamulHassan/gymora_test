using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gymora.Models.ViewModels.Member
{
    public class MemberEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(15)]
        [Display(Name = "CNIC")]
        public string? CNIC { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(200)]
        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [StringLength(1000)]
        [Display(Name = "Health Conditions")]
        public string? HealthConditions { get; set; }

        [StringLength(500)]
        [Display(Name = "Fitness Goals")]
        public string? FitnessGoals { get; set; }

        public bool IsActive { get; set; }
        [Display(Name = "Membership Plan")]
        public Guid? CurrentPlanId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Plan Start Date")]
        public DateTime? PlanStartDate { get; set; }
        public List<SelectListItem> PlanOptions { get; set; } = new();

        // Read-only display field — not posted
        public string? Email { get; set; }
    }
}
