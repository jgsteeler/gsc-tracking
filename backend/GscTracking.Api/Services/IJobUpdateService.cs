using GscTracking.Api.DTOs;

namespace GscTracking.Api.Services;

public interface IJobUpdateService
{
    Task<IEnumerable<JobUpdateDto>> GetJobUpdatesAsync(int jobId);
    Task<JobUpdateDto?> GetJobUpdateByIdAsync(int id);
    Task<JobUpdateDto> CreateJobUpdateAsync(int jobId, JobUpdateRequestDto updateRequest);
    Task<bool> DeleteJobUpdateAsync(int id);
}
