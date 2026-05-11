namespace Gymora.Models.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
        public int MembersJoinedThisMonth { get; set; }

        /// <summary>
        /// Counts of members who joined in each of the last 12 months,
        /// ordered oldest → newest. Built by DashboardController.
        /// </summary>
        public List<MonthlyJoinCount> MonthlyJoins { get; set; } = new();
    }

    public class MonthlyJoinCount
    {
        /// <summary>Display label, e.g. "Jan 25"</summary>
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
