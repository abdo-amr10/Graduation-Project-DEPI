using My_Uni_Hub.Models.Pages;
using System.Threading;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface IOpportunityService
    {
        Task<List<Opportunity>> GetAllAsync(CancellationToken ct = default);
        Task<Opportunity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Opportunity> CreateAsync(Opportunity opportunity, CancellationToken ct = default);
        Task UpdateAsync(Opportunity opportunity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
