using Gymora.Data.Entities;
using Gymora.Models.ViewModels.Member;
using Gymora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gymora.Areas.Member.Controllers
{
    [Authorize(Roles = "Member")]
    [Area("Member")]
    public class PortalController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PortalController(IMemberService memberService, UserManager<ApplicationUser> userManager)
        {
            _memberService = memberService;
            _userManager = userManager;
        }

        // GET /Member/Portal/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var model = await _memberService.GetOwnProfileAsync(user.Id);
            if (model == null) return NotFound();
            return View(model);
        }

        // GET /Member/Portal/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var tenantId = user.TenantId ?? new Guid("11111111-1111-1111-1111-111111111111");
            var model = await _memberService.GetMemberForEditAsync(user.Id, tenantId);
            if (model == null) return NotFound();

            return View(model);
        }

        // POST /Member/Portal/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(MemberEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            model.Id = user.Id;

            var (success, error) = await _memberService.UpdateOwnProfileAsync(model, user.Id);
            if (success)
            {
                TempData["Success"] = "Profile updated.";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }
    }
}
