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

    public async Task<IEnumerable<Job>> GetJobsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(j => j.CustomerId == customerId)
            .Include(j => j.Customer)
            .Include(j => j.Expenses)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetJobsByStatusAsync(JobStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(j => j.Status == status)
            .Include(j => j.Customer)
            .Include(j => j.Expenses)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync(cancellationToken);
    }

    public async Task<Job?> GetJobWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(j => j.Customer)
            .Include(j => j.JobUpdates)
            .Include(j => j.Expenses)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public override async Task<Job?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(j => j.Customer)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(j => j.Customer)
            .Include(j => j.Expenses)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync(cancellationToken);
    }
}
