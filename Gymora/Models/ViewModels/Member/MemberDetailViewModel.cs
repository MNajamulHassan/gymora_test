namespace Gymora.Models.ViewModels.Member
{
    public class MemberDetailViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CNIC { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? HealthConditions { get; set; }
        public string? FitnessGoals { get; set; }
        public bool IsActive { get; set; }
        public DateTime? JoinDate { get; set; }
        public string? CurrentPlanName { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Role { get; set; }

        public string Initials
        {
            get
            {
                var words = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var initials = words
                    .Take(2)
                    .Select(w => char.ToUpper(w[0]))
                    .ToArray();
                return new string(initials);
            }
        }
    }
}
