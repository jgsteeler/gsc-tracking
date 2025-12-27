using Microsoft.EntityFrameworkCore;
using GscTracking.Core.Entities;
using GscTracking.Core.Interfaces;
using GscTracking.Infrastructure.Data;

namespace GscTracking.Infrastructure.Repositories;

public class JobUpdateRepository : Repository<JobUpdate>, IJobUpdateRepository
{
    public JobUpdateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<JobUpdate>> GetUpdatesByJobIdAsync(int jobId)
    {
        return await _dbSet
            .Where(u => u.JobId == jobId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }
}
