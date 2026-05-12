using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public string DurationPreset { get; set; } = "custom";

        [Display(Name = "Assign Trainer (optional)")]
        public Guid? AssignedTrainerId { get; set; }

        [Range(1, 20)]
        [Display(Name = "Max Members")]
        public int MaxMembers { get; set; } = 1;

        [Range(0, 999999)]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> ActiveTrainers { get; set; } = new();
    }
}
