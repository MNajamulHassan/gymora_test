using System.ComponentModel.DataAnnotations;

namespace Gymora.Models.ViewModels.Plan
{
    public class PlanFormViewModel
    {
        public Guid? PlanId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; } = string.Empty;

        [Range(1, 3650)]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Range(0, 999999)]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
