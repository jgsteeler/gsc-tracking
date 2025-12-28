using Microsoft.EntityFrameworkCore;
using GscTracking.Core.Entities;
using GscTracking.Core.Interfaces;
using GscTracking.Infrastructure.Data;

namespace GscTracking.Infrastructure.Repositories;

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Expense>> GetExpensesByJobIdAsync(int jobId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.JobId == jobId)
            .OrderByDescending(e => e.Date)
            .ToListAsync(cancellationToken);
    }
}
