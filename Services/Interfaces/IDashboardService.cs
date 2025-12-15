using My_Uni_Hub.Models.ViewModels.UserViewModel;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardForUserAsync(string userId, CancellationToken ct = default);

    }
}
