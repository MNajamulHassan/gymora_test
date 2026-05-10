using Gymora.Models.ViewModels.Plan;

namespace Gymora.Services.Interfaces
{
    public interface IPlanService
    {
        Task<PlanListViewModel> GetPlansAsync(Guid tenantId);
        Task<PlanFormViewModel> GetCreateModelAsync();
        Task<PlanFormViewModel?> GetEditModelAsync(Guid planId, Guid tenantId);
        Task<(bool Success, string Error)> CreateAsync(PlanFormViewModel model, Guid tenantId);
        Task<(bool Success, string Error)> UpdateAsync(PlanFormViewModel model, Guid tenantId);
        Task<(bool Success, string Error)> ToggleActiveAsync(Guid planId, Guid tenantId);
        Task<(bool Success, string Error)> DeleteAsync(Guid planId, Guid tenantId);
    }
}