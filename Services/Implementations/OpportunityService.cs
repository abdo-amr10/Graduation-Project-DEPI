using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class OpportunityService : IOpportunityService
    {
        private readonly MyUniDbContext _db;
        public OpportunityService(MyUniDbContext db) => _db = db;

        public async Task<List<Opportunity>> GetAllAsync(CancellationToken ct = default)
            => await _db.Set<Opportunity>()
                        .AsNoTracking()
                        .Include(o => o.Faculty)
                        .Include(o => o.Department)
                        .OrderByDescending(o => o.PostedAt)
                        .ToListAsync(ct);

        public async Task<Opportunity?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Set<Opportunity>().FindAsync(new object[] { id }, ct);

        public async Task<Opportunity> CreateAsync(Opportunity opportunity, CancellationToken ct = default)
        {
            try
            {
                opportunity.PostedAt = opportunity.PostedAt == default ? DateTime.UtcNow : opportunity.PostedAt;

                _db.Opportunities.Add(opportunity);
                await _db.SaveChangesAsync(ct);

                System.Diagnostics.Debug.WriteLine("DEBUG: SaveChanges completed for Opportunity Id=" + opportunity.OpportunityId);

                return opportunity;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in CreateAsync: " + ex.ToString());
                throw;
            }
        }


        public async Task UpdateAsync(Opportunity opportunity, CancellationToken ct = default)
        {
            _db.Opportunities.Update(opportunity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.Opportunities.FindAsync(new object[] { id }, ct);
            if (e != null)
            {
                _db.Opportunities.Remove(e);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}