namespace Gymora.Models.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
        public int MembersJoinedThisMonth { get; set; }
    }
}
