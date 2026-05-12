using Gymora.Data;
using Gymora.Data.Entities;
using Gymora.Models.ViewModels.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymora.Controllers
{
    [Authorize(Roles = "GymOwner")]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static readonly Guid DemoTenantId = new Guid("11111111-1111-1111-1111-111111111111");

        public TrainerController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var trainers = await _db.Trainers
                .Where(t => t.TenantId == DemoTenantId)
                .OrderByDescending(t => t.IsActive)
                .ThenBy(t => t.FullName)
                .ToListAsync();
            return View(trainers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TrainerFormViewModel { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var trainer = new Trainer
            {
                TrainerId = Guid.NewGuid(),
                TenantId = DemoTenantId,
                FullName = model.FullName.Trim(),
                Specialization = model.Specialization?.Trim(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                IsActive = model.IsActive
            };

            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Trainer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var trainer = await _db.Trainers
                .FirstOrDefaultAsync(t => t.TrainerId == id && t.TenantId == DemoTenantId);
            if (trainer == null) return NotFound();

            return View(new TrainerFormViewModel
            {
                TrainerId = trainer.TrainerId,
                FullName = trainer.FullName,
                Specialization = trainer.Specialization,
                PhoneNumber = trainer.PhoneNumber,
                IsActive = trainer.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TrainerFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var trainer = await _db.Trainers
                .FirstOrDefaultAsync(t => t.TrainerId == id && t.TenantId == DemoTenantId);
            if (trainer == null) return NotFound();

            trainer.FullName = model.FullName.Trim();
            trainer.Specialization = model.Specialization?.Trim();
            trainer.PhoneNumber = model.PhoneNumber?.Trim();
            trainer.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Trainer updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var trainer = await _db.Trainers
                .FirstOrDefaultAsync(t => t.TrainerId == id && t.TenantId == DemoTenantId);
            if (trainer == null)
            {
                TempData["Error"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }

            trainer.IsActive = !trainer.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Trainer status updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
