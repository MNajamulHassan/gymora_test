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
            var membersInRole = await _userManager.GetUsersInRoleAsync("Member");
            var tenantMembers = membersInRole
                .Where(u => u.TenantId == DemoTenantId && !u.IsDeleted)
                .ToList();

            var now = DateTime.Today;
            var membersJoinedThisMonth = await _db.Users
                .Where(u =>
                    u.TenantId == DemoTenantId &&
                    !u.IsDeleted &&
                    u.JoinDate.HasValue &&
                    u.JoinDate.Value.Month == now.Month &&
                    u.JoinDate.Value.Year == now.Year)
                .CountAsync();

            var model = new AdminDashboardViewModel
            {
                TotalMembers = tenantMembers.Count,
                ActiveMembers = tenantMembers.Count(m => m.IsActive),
                InactiveMembers = tenantMembers.Count(m => !m.IsActive),
                MembersJoinedThisMonth = membersJoinedThisMonth
            };

            return View(model);
        }
    }
}
