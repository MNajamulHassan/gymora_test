using Gymora.Models.ViewModels.Member;
using Gymora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymora.Controllers
{
    [Authorize(Roles = "GymOwner,Receptionist")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private static readonly Guid DemoTenantId = new Guid("11111111-1111-1111-1111-111111111111");

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // GET /Member/Index
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var model = await _memberService.GetMemberListAsync(DemoTenantId, search, page, 20);
            return View(model);
        }

        // GET /Member/Detail/{id}
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var model = await _memberService.GetMemberDetailAsync(id, DemoTenantId);
            if (model == null) return NotFound();
            return View(model);
        }

        // GET /Member/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new MemberCreateViewModel());
        }

        // POST /Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _memberService.CreateMemberManuallyAsync(model, DemoTenantId);
            if (success)
            {
                TempData["Success"] = "Member registered successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        // GET /Member/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _memberService.GetMemberForEditAsync(id, DemoTenantId);
            if (model == null) return NotFound();
            await _memberService.PopulatePlanOptionsAsync(model, DemoTenantId);
            return View(model);
        }

        // POST /Member/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MemberEditViewModel model)
        {
            model.Id = id;
            await _memberService.PopulatePlanOptionsAsync(model, DemoTenantId);

            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _memberService.UpdateMemberAsync(model, DemoTenantId);
            if (success)
                return RedirectToAction(nameof(Detail), new { id });

            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        // POST /Member/Deactivate/{id}
        [HttpPost]
        [Authorize(Roles = "GymOwner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            var (success, error) = await _memberService.DeactivateMemberAsync(id, DemoTenantId);
            if (success)
                TempData["Success"] = "Member has been deactivated.";
            else
                TempData["Error"] = error;

            return RedirectToAction(nameof(Detail), new { id });
        }

        // POST /Member/Reactivate/{id}
        [HttpPost]
        [Authorize(Roles = "GymOwner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(string id)
        {
            var (success, error) = await _memberService.ReactivateMemberAsync(id, DemoTenantId);
            if (success)
                TempData["Success"] = "Member has been reactivated.";
            else
                TempData["Error"] = error;

            return RedirectToAction(nameof(Detail), new { id });
        }

        // POST /Member/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "GymOwner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var (success, error) = await _memberService.DeleteMemberAsync(id, DemoTenantId);
            if (success)
            {
                TempData["Success"] = "Member has been permanently deleted.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = error;
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}