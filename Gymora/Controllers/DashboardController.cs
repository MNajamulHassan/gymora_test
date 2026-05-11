using Gymora.Data;
using Gymora.Data.Entities;
using Gymora.Models.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymora.Controllers
{
    [Authorize(Roles = "GymOwner,Receptionist")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly Guid DemoTenantId = new Guid("11111111-1111-1111-1111-111111111111");

        public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // ── Member counts ─────────────────────────────────────────────
            var membersInRole = await _userManager.GetUsersInRoleAsync("Member");
            var tenantMembers = membersInRole
                .Where(u => u.TenantId == DemoTenantId && !u.IsDeleted)
                .ToList();

            var now = DateTime.Today;
            var membersJoinedThisMonth = tenantMembers.Count(u =>
                u.JoinDate.HasValue &&
                u.JoinDate.Value.Month == now.Month &&
                u.JoinDate.Value.Year == now.Year);

            // ── Last 12 months of join activity (real DB data) ────────────
            // Build the 12 month buckets ending this month
            var monthBuckets = Enumerable.Range(0, 12)
                .Select(i => now.AddMonths(-11 + i))   // oldest first
                .Select(d => new { d.Year, d.Month })
                .ToList();

            // Pull all join dates for tenant members in one go (already in memory)
            var joinDates = tenantMembers
                .Where(u => u.JoinDate.HasValue)
                .Select(u => u.JoinDate!.Value)
                .ToList();

            var monthlyJoins = monthBuckets.Select(b => new MonthlyJoinCount
            {
                Label = new DateTime(b.Year, b.Month, 1).ToString("MMM yy"),
                Count = joinDates.Count(d => d.Year == b.Year && d.Month == b.Month)
            }).ToList();

            var model = new AdminDashboardViewModel
            {
                TotalMembers         = tenantMembers.Count,
                ActiveMembers        = tenantMembers.Count(m => m.IsActive),
                InactiveMembers      = tenantMembers.Count(m => !m.IsActive),
                MembersJoinedThisMonth = membersJoinedThisMonth,
                MonthlyJoins         = monthlyJoins
            };

            return View(model);
        }
    }
}
