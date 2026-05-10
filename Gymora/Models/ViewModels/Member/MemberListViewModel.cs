namespace Gymora.Models.ViewModels.Member
{
    public class MemberListViewModel
    {
        public List<MemberRowViewModel> Members { get; set; } = new();
        public string? SearchQuery { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class MemberRowViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool IsActive { get; set; }
        public string StatusBadge => IsActive ? "Active" : "Inactive";
        public string StatusClass => IsActive ? "success" : "danger";
    }
}
