using Microsoft.EntityFrameworkCore;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;

namespace GscTracking.Api.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync(string? searchTerm = null, string? statusFilter = null)
    {
        var query = _context.Job.Include(j => j.Customer).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(j =>
                j.EquipmentType.ToLower().Contains(searchTerm) ||
                j.EquipmentModel.ToLower().Contains(searchTerm) ||
                j.Description.ToLower().Contains(searchTerm) ||
                j.Customer.Name.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            if (Enum.TryParse<JobStatus>(statusFilter, ignoreCase: true, out var status))
            {
                query = query.Where(j => j.Status == status);
            }
        }

        var jobs = await query
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync();

        return jobs.Select(j => MapToDto(j));
    }

    public async Task<JobDto?> GetJobByIdAsync(int id)
    {
        var job = await _context.Job
            .Include(j => j.Customer)
            .FirstOrDefaultAsync(j => j.Id == id);
        
        if (job == null)
        {
            return null;
        }

        return MapToDto(job);
    }

    public async Task<IEnumerable<JobDto>> GetJobsByCustomerIdAsync(int customerId)
    {
        var jobs = await _context.Job
            .Include(j => j.Customer)
            .Where(j => j.CustomerId == customerId)
            .OrderByDescending(j => j.DateReceived)
            .ToListAsync();

        return jobs.Select(j => MapToDto(j));
    }

    public async Task<JobDto> CreateJobAsync(JobRequestDto jobRequest)
    {
        // Parse the status string to enum
        if (!Enum.TryParse<JobStatus>(jobRequest.Status, ignoreCase: true, out var status))
        {
            throw new ArgumentException($"Invalid status: {jobRequest.Status}");
        }

        var job = new Job
        {
            CustomerId = jobRequest.CustomerId,
            EquipmentType = jobRequest.EquipmentType,
            EquipmentModel = jobRequest.EquipmentModel,
            Description = jobRequest.Description,
            Status = status,
            DateReceived = jobRequest.DateReceived,
            DateCompleted = jobRequest.DateCompleted,
            EstimateAmount = jobRequest.EstimateAmount,
            ActualAmount = jobRequest.ActualAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Job.Add(job);
        await _context.SaveChangesAsync();

        // Load the customer for the DTO
        await _context.Entry(job).Reference(j => j.Customer).LoadAsync();

        return MapToDto(job);
    }

    public async Task<JobDto?> UpdateJobAsync(int id, JobRequestDto jobRequest)
    {
        var job = await _context.Job
            .Include(j => j.Customer)
            .FirstOrDefaultAsync(j => j.Id == id);
        
        if (job == null)
        {
            return null;
        }

        // Parse the status string to enum
        if (!Enum.TryParse<JobStatus>(jobRequest.Status, ignoreCase: true, out var status))
        {
            throw new ArgumentException($"Invalid status: {jobRequest.Status}");
        }

        job.CustomerId = jobRequest.CustomerId;
        job.EquipmentType = jobRequest.EquipmentType;
        job.EquipmentModel = jobRequest.EquipmentModel;
        job.Description = jobRequest.Description;
        job.Status = status;
        job.DateReceived = jobRequest.DateReceived;
        job.DateCompleted = jobRequest.DateCompleted;
        job.EstimateAmount = jobRequest.EstimateAmount;
        job.ActualAmount = jobRequest.ActualAmount;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task<bool> DeleteJobAsync(int id)
    {
        var job = await _context.Job
            .FirstOrDefaultAsync(j => j.Id == id);
        
        if (job == null)
        {
            return false;
        }

        // Soft delete
        job.IsDeleted = true;
        job.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    private static JobDto MapToDto(Job job)
    {
        return new JobDto
        {
            Id = job.Id,
            CustomerId = job.CustomerId,
            CustomerName = job.Customer.Name,
            EquipmentType = job.EquipmentType,
            EquipmentModel = job.EquipmentModel,
            Description = job.Description,
            Status = job.Status.ToString(),
            DateReceived = job.DateReceived,
            DateCompleted = job.DateCompleted,
            EstimateAmount = job.EstimateAmount,
            ActualAmount = job.ActualAmount,
            CreatedAt = job.CreatedAt,
            UpdatedAt = job.UpdatedAt
        };
    }
}
