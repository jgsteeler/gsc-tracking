using GscTracking.Api.DTOs;

namespace GscTracking.Api.Services;

public interface IJobService
{
    Task<IEnumerable<JobDto>> GetAllJobsAsync(string? searchTerm = null, string? statusFilter = null);
    Task<JobDto?> GetJobByIdAsync(int id);
    Task<IEnumerable<JobDto>> GetJobsByCustomerIdAsync(int customerId);
    Task<JobDto> CreateJobAsync(JobRequestDto jobRequest);
    Task<JobDto?> UpdateJobAsync(int id, JobRequestDto jobRequest);
    Task<bool> DeleteJobAsync(int id);
}
