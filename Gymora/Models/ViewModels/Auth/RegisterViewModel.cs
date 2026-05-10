using System.ComponentModel.DataAnnotations;

namespace Gymora.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(500)]
        [Display(Name = "Fitness Goals")]
        public string? FitnessGoals { get; set; }

        [StringLength(1000)]
        [Display(Name = "Health Conditions")]
        public string? HealthConditions { get; set; }
    }
}
