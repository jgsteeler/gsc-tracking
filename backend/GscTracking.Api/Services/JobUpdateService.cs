using Microsoft.EntityFrameworkCore;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;

namespace GscTracking.Api.Services;

public class JobUpdateService : IJobUpdateService
{
    private readonly ApplicationDbContext _context;

    public JobUpdateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobUpdateDto>> GetJobUpdatesAsync(int jobId)
    {
        var updates = await _context.JobUpdate
            .Where(u => u.JobId == jobId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return updates.Select(u => MapToDto(u));
    }

    public async Task<JobUpdateDto?> GetJobUpdateByIdAsync(int id)
    {
        var update = await _context.JobUpdate
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (update == null)
        {
            return null;
        }

        return MapToDto(update);
    }

    public async Task<JobUpdateDto> CreateJobUpdateAsync(int jobId, JobUpdateRequestDto updateRequest)
    {
        // Verify that the job exists
        var job = await _context.Job.FindAsync(jobId);
        if (job == null)
        {
            throw new ArgumentException($"Job with ID {jobId} not found.");
        }

        var jobUpdate = new JobUpdate
        {
            JobId = jobId,
            UpdateText = updateRequest.UpdateText,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobUpdate.Add(jobUpdate);
        await _context.SaveChangesAsync();

        return MapToDto(jobUpdate);
    }

    public async Task<bool> DeleteJobUpdateAsync(int id)
    {
        var update = await _context.JobUpdate
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (update == null)
        {
            return false;
        }

        _context.JobUpdate.Remove(update);
        await _context.SaveChangesAsync();

        return true;
    }

    private static JobUpdateDto MapToDto(JobUpdate update)
    {
        return new JobUpdateDto
        {
            Id = update.Id,
            JobId = update.JobId,
            UpdateText = update.UpdateText,
            CreatedAt = update.CreatedAt
        };
    }
}
