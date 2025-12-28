using GscTracking.Core.Entities;

namespace GscTracking.Core.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IEnumerable<Expense>> GetExpensesByJobIdAsync(int jobId, CancellationToken cancellationToken = default);
}
