using Gymora.Models.ViewModels.Plan;
using Gymora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymora.Controllers
{
    [Authorize(Roles = "GymOwner")]
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;
        private static readonly Guid DemoTenantId = new Guid("11111111-1111-1111-1111-111111111111");

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _planService.GetPlansAsync(DemoTenantId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _planService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _planService.CreateAsync(model, DemoTenantId);
            if (success)
            {
                TempData["Success"] = "Plan created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _planService.GetEditModelAsync(id, DemoTenantId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PlanFormViewModel model)
        {
            model.PlanId = id;

            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _planService.UpdateAsync(model, DemoTenantId);
            if (success)
            {
                TempData["Success"] = "Plan updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var (success, error) = await _planService.ToggleActiveAsync(id, DemoTenantId);
            if (success)
                TempData["Success"] = "Plan status updated.";
            else
                TempData["Error"] = error;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (success, error) = await _planService.DeleteAsync(id, DemoTenantId);
            if (success)
                TempData["Success"] = "Plan deleted successfully.";
            else
                TempData["Error"] = error;

            return RedirectToAction(nameof(Index));
        }
    }
}