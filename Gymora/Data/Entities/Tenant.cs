namespace Gymora.Data.Entities
{
    public class Tenant
    {
        public Guid TenantId { get; set; } = Guid.NewGuid();
        public string GymName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public int GracePeriodDays { get; set; } = 3;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
