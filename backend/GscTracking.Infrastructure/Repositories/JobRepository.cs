using Microsoft.EntityFrameworkCore;
using GscTracking.Core.Entities;
using GscTracking.Core.Enums;
using GscTracking.Core.Interfaces;
using GscTracking.Infrastructure.Data;

namespace GscTracking.Infrastructure.Repositories;

public class JobRepository : Repository<Job>, IJobRepository
{
    public JobRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Job>> GetJobsByCustomerIdAsync(int customerId)
    {
        return await _dbSet
            .Where(j => j.CustomerId == customerId)
            .Include(j => j.Customer)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetJobsByStatusAsync(JobStatus status)
    {
        return await _dbSet
            .Where(j => j.Status == status)
            .Include(j => j.Customer)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync();
    }

    public async Task<Job?> GetJobWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(j => j.Customer)
            .Include(j => j.JobUpdates)
            .Include(j => j.Expenses)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public override async Task<Job?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(j => j.Customer)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public override async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _dbSet
            .Include(j => j.Customer)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync();
    }
}
