using GscTracking.Core.Entities;

namespace GscTracking.Core.Interfaces;

public interface IJobUpdateRepository : IRepository<JobUpdate>
{
    Task<IEnumerable<JobUpdate>> GetUpdatesByJobIdAsync(int jobId, CancellationToken cancellationToken = default);
}
