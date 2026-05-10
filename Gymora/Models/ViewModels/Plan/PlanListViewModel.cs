namespace Gymora.Models.ViewModels.Plan
{
    public class PlanListViewModel
    {
        public List<PlanRowViewModel> Plans { get; set; } = new();
    }

    public class PlanRowViewModel
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
