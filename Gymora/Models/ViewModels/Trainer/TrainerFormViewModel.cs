using System.ComponentModel.DataAnnotations;

namespace Gymora.Models.ViewModels.Trainer
{
    public class TrainerFormViewModel
    {
        public Guid? TrainerId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Specialization { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
