using Gymora.Data;
using Gymora.Data.Entities;
using Gymora.Models.ViewModels.Member;
using Gymora.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gymora.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MemberService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<MemberListViewModel> GetMemberListAsync(Guid tenantId, string? search, int page, int pageSize)
        {
            // Get all users in "Member" role for this tenant
            var membersInRole = await _userManager.GetUsersInRoleAsync("Member");

            var query = membersInRole
                .Where(u => u.TenantId == tenantId && !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(lower) ||
                    (u.Email != null && u.Email.ToLower().Contains(lower)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(lower)));
            }

            var ordered = query.OrderBy(u => u.FullName).ToList();
            var totalCount = ordered.Count;
            var paged = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var rows = paged.Select(u => new MemberRowViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                PhoneNumber = u.PhoneNumber,
                Gender = u.Gender,
                JoinDate = u.JoinDate,
                IsActive = u.IsActive
            }).ToList();

            return new MemberListViewModel
            {
                Members = rows,
                SearchQuery = search,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<MemberDetailViewModel?> GetMemberDetailAsync(string userId, Guid tenantId)
        {
            var user = await _db.Users
                .Include(u => u.CurrentPlan)
                .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && !u.IsDeleted);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return MapToDetail(user, roles.FirstOrDefault());
        }

        public async Task<(bool Success, string Error)> CreateMemberManuallyAsync(MemberCreateViewModel model, Guid tenantId)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return (false, "A user with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                CNIC = model.CNIC,
                Address = model.Address,
                EmergencyContact = model.EmergencyContact,
                HealthConditions = model.HealthConditions,
                FitnessGoals = model.FitnessGoals,
                TenantId = tenantId,
                JoinDate = DateTime.Today,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            await _userManager.AddToRoleAsync(user, "Member");
            return (true, string.Empty);
        }

        public async Task<MemberEditViewModel?> GetMemberForEditAsync(string userId, Guid tenantId)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && !u.IsDeleted);

            if (user == null) return null;

            return new MemberEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                CNIC = user.CNIC,
                Address = user.Address,
                EmergencyContact = user.EmergencyContact,
                HealthConditions = user.HealthConditions,
                FitnessGoals = user.FitnessGoals,
                IsActive = user.IsActive,
                Email = user.Email,
                CurrentPlanId = user.CurrentPlanId,
                PlanStartDate = user.PlanStartDate
            };
        }

        public async Task<(bool Success, string Error)> UpdateMemberAsync(MemberEditViewModel model, Guid tenantId)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == model.Id && u.TenantId == tenantId && !u.IsDeleted);

            if (user == null)
                return (false, "Member not found.");

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.CNIC = model.CNIC;
            user.Address = model.Address;
            user.EmergencyContact = model.EmergencyContact;
            user.HealthConditions = model.HealthConditions;
            user.FitnessGoals = model.FitnessGoals;
            user.IsActive = model.IsActive;
            user.CurrentPlanId = model.CurrentPlanId;
            user.PlanStartDate = model.CurrentPlanId.HasValue
                ? (model.PlanStartDate ?? DateTime.Today)
                : null;
            user.PlanExpiryDate = null;

            if (model.CurrentPlanId.HasValue && user.PlanStartDate.HasValue)
            {
                var plan = await _db.MembershipPlans.FirstOrDefaultAsync(p =>
                    p.PlanId == model.CurrentPlanId.Value &&
                    p.TenantId == tenantId &&
                    p.IsActive);

                if (plan == null)
                    return (false, "Selected membership plan is not available.");

                user.PlanExpiryDate = user.PlanStartDate.Value.AddDays(plan.DurationDays);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> DeactivateMemberAsync(string userId, Guid tenantId)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);

            if (user == null)
                return (false, "Member not found.");

            user.IsActive = false;
            user.IsDeleted = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, string.Empty);
        }

        public async Task<MemberDetailViewModel?> GetOwnProfileAsync(string userId)
        {
            var user = await _db.Users
                .Include(u => u.CurrentPlan)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return MapToDetail(user, roles.FirstOrDefault());
        }

        public async Task<(bool Success, string Error)> UpdateOwnProfileAsync(MemberEditViewModel model, string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
                return (false, "User not found.");

            // Only allow safe fields — no email, TenantId, IsActive, roles, or IsDeleted changes
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.EmergencyContact = model.EmergencyContact;
            user.HealthConditions = model.HealthConditions;
            user.FitnessGoals = model.FitnessGoals;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, string.Empty);
        }

        public async Task PopulatePlanOptionsAsync(MemberEditViewModel model, Guid tenantId)
        {
            var plans = await _db.MembershipPlans
                .Where(p => p.TenantId == tenantId && p.IsActive)
                .OrderBy(p => p.PlanName)
                .Select(p => new SelectListItem
                {
                    Value = p.PlanId.ToString(),
                    Text = $"{p.PlanName} ({p.DurationDays} days)"
                })
                .ToListAsync();

            model.PlanOptions = new List<SelectListItem>
            {
                new() { Value = string.Empty, Text = "-- No Plan --" }
            };
            model.PlanOptions.AddRange(plans);
        }

        // ─── Private helpers ───────────────────────────────────────────────
        private static MemberDetailViewModel MapToDetail(ApplicationUser user, string? role) =>
            new MemberDetailViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                CNIC = user.CNIC,
                Address = user.Address,
                EmergencyContact = user.EmergencyContact,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                HealthConditions = user.HealthConditions,
                FitnessGoals = user.FitnessGoals,
                IsActive = user.IsActive,
                JoinDate = user.JoinDate,
                CurrentPlanName = user.CurrentPlan?.PlanName,
                PlanStartDate = user.PlanStartDate,
                PlanExpiryDate = user.PlanExpiryDate,
                CreatedAt = user.CreatedAt,
                Role = role
            };
    }
}
