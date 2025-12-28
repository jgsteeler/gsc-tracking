using GscTracking.Core.Entities;
using GscTracking.Core.Enums;

namespace GscTracking.Core.Interfaces;

public interface IJobRepository : IRepository<Job>
{
    Task<IEnumerable<Job>> GetJobsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetJobsByStatusAsync(JobStatus status, CancellationToken cancellationToken = default);
    Task<Job?> GetJobWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
