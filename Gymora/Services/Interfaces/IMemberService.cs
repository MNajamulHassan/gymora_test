using Gymora.Models.ViewModels.Member;

namespace Gymora.Services.Interfaces
{
    public interface IMemberService
    {
        Task<MemberListViewModel> GetMemberListAsync(Guid tenantId, string? search, int page, int pageSize, string? sortBy = null, string? sortDir = null, string? statusFilter = null);
        Task<MemberDetailViewModel?> GetMemberDetailAsync(string userId, Guid tenantId);
        Task<(bool Success, string Error)> CreateMemberManuallyAsync(MemberCreateViewModel model, Guid tenantId);
        Task<MemberEditViewModel?> GetMemberForEditAsync(string userId, Guid tenantId);
        Task<(bool Success, string Error)> UpdateMemberAsync(MemberEditViewModel model, Guid tenantId);
        Task<(bool Success, string Error)> DeactivateMemberAsync(string userId, Guid tenantId);
        Task<(bool Success, string Error)> ReactivateMemberAsync(string userId, Guid tenantId);
        Task<(bool Success, string Error)> DeleteMemberAsync(string userId, Guid tenantId);
        Task<MemberDetailViewModel?> GetOwnProfileAsync(string userId);
        Task<(bool Success, string Error)> UpdateOwnProfileAsync(MemberEditViewModel model, string userId);
        Task PopulatePlanOptionsAsync(MemberEditViewModel model, Guid tenantId);
    }
}