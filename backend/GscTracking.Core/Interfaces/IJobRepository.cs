using GscTracking.Core.Entities;
using GscTracking.Core.Enums;

namespace GscTracking.Core.Interfaces;

public interface IJobRepository : IRepository<Job>
{
    Task<IEnumerable<Job>> GetJobsByCustomerIdAsync(int customerId);
    Task<IEnumerable<Job>> GetJobsByStatusAsync(JobStatus status);
    Task<Job?> GetJobWithDetailsAsync(int id);
}
