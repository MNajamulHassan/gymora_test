using Gymora.Data;
using Gymora.Data.Entities;
using Gymora.Models.ViewModels.Plan;
using Gymora.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gymora.Services
{
    public class PlanService : IPlanService
    {
        private readonly ApplicationDbContext _db;

        public PlanService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PlanListViewModel> GetPlansAsync(Guid tenantId, string? statusFilter = null)
        {
            var plans = await _db.MembershipPlans
                .Where(p => p.TenantId == tenantId)
                .OrderByDescending(p => p.IsActive)
                .ThenBy(p => p.PlanName)
                .Select(p => new PlanRowViewModel
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    DurationDays = p.DurationDays,
                    Price = p.Price,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            if (statusFilter == "active")
                plans = plans.Where(p => p.IsActive).ToList();
            else if (statusFilter == "inactive")
                plans = plans.Where(p => !p.IsActive).ToList();

            return new PlanListViewModel { Plans = plans, StatusFilter = statusFilter };
        }

        public async Task<PlanFormViewModel> GetCreateModelAsync(Guid tenantId)
        {
            return new PlanFormViewModel
            {
                IsActive = true,
                DurationDays = 30,
                DurationPreset = "30",
                MaxMembers = 1,
                ActiveTrainers = await GetActiveTrainerSelectListAsync(tenantId)
            };
        }

        public async Task<PlanFormViewModel?> GetEditModelAsync(Guid planId, Guid tenantId)
        {
            var model = await _db.MembershipPlans
                .Where(p => p.PlanId == planId && p.TenantId == tenantId)
                .Select(p => new PlanFormViewModel
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    DurationDays = p.DurationDays,
                    DurationPreset = GetDurationPreset(p.DurationDays),
                    AssignedTrainerId = p.AssignedTrainerId,
                    MaxMembers = p.MaxMembers,
                    Price = p.Price,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    ActiveTrainers = new List<SelectListItem>()
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.ActiveTrainers = await GetActiveTrainerSelectListAsync(tenantId);
            }

            return model;
        }

        public async Task<(bool Success, string Error)> CreateAsync(PlanFormViewModel model, Guid tenantId)
        {
            var duplicateExists = await _db.MembershipPlans.AnyAsync(p =>
                p.TenantId == tenantId &&
                p.PlanName == model.PlanName);

            if (duplicateExists)
                return (false, "A plan with this name already exists.");

            var plan = new MembershipPlan
            {
                PlanId = Guid.NewGuid(),
                TenantId = tenantId,
                PlanName = model.PlanName.Trim(),
                DurationDays = model.DurationDays,
                AssignedTrainerId = model.AssignedTrainerId,
                MaxMembers = model.MaxMembers,
                Price = model.Price,
                Description = model.Description?.Trim(),
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.MembershipPlans.Add(plan);
            await _db.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> UpdateAsync(PlanFormViewModel model, Guid tenantId)
        {
            if (!model.PlanId.HasValue)
                return (false, "Plan id is required.");

            var plan = await _db.MembershipPlans
                .FirstOrDefaultAsync(p => p.PlanId == model.PlanId.Value && p.TenantId == tenantId);

            if (plan == null)
                return (false, "Plan not found.");

            var duplicateExists = await _db.MembershipPlans.AnyAsync(p =>
                p.TenantId == tenantId &&
                p.PlanId != plan.PlanId &&
                p.PlanName == model.PlanName);

            if (duplicateExists)
                return (false, "A plan with this name already exists.");

            plan.PlanName = model.PlanName.Trim();
            plan.DurationDays = model.DurationDays;
            plan.AssignedTrainerId = model.AssignedTrainerId;
            plan.MaxMembers = model.MaxMembers;
            plan.Price = model.Price;
            plan.Description = model.Description?.Trim();
            plan.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> ToggleActiveAsync(Guid planId, Guid tenantId)
        {
            var plan = await _db.MembershipPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId && p.TenantId == tenantId);

            if (plan == null)
                return (false, "Plan not found.");

            plan.IsActive = !plan.IsActive;
            await _db.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> DeleteAsync(Guid planId, Guid tenantId)
        {
            var plan = await _db.MembershipPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId && p.TenantId == tenantId);

            if (plan == null)
                return (false, "Plan not found.");

            var membersOnPlan = await _db.Users
                .AnyAsync(u => u.CurrentPlanId == planId && !u.IsDeleted);

            if (membersOnPlan)
                return (false, "Cannot delete this plan — it is currently assigned to one or more members.");

            _db.MembershipPlans.Remove(plan);
            await _db.SaveChangesAsync();

            return (true, string.Empty);
        }

        private async Task<List<SelectListItem>> GetActiveTrainerSelectListAsync(Guid tenantId)
        {
            return await _db.Trainers
                .Where(t => t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.FullName)
                .Select(t => new SelectListItem
                {
                    Value = t.TrainerId.ToString(),
                    Text = t.FullName
                })
                .ToListAsync();
        }

        private static string GetDurationPreset(int days) => days switch
        {
            1 => "1",
            7 => "7",
            30 => "30",
            180 => "180",
            365 => "365",
            _ => "custom"
        };
    }
}