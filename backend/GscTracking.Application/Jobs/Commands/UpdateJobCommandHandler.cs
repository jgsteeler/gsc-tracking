using GscTracking.Application.DTOs;
using GscTracking.Core.Enums;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Jobs.Commands;

public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, JobDto?>
{
    private readonly IJobRepository _jobRepository;

    public UpdateJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<JobDto?> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetJobWithDetailsAsync(request.Id, cancellationToken);
        
        if (job == null)
        {
            return null;
        }

        // Parse status from string
        if (Enum.TryParse<JobStatus>(request.JobRequest.Status, true, out var status))
        {
            job.Status = status;
        }

        job.CustomerId = request.JobRequest.CustomerId;
        job.EquipmentType = request.JobRequest.EquipmentType;
        job.EquipmentModel = request.JobRequest.EquipmentModel;
        job.Description = request.JobRequest.Description;
        job.DateReceived = request.JobRequest.DateReceived;
        job.DateCompleted = request.JobRequest.DateCompleted;
        job.EstimateAmount = request.JobRequest.EstimateAmount;
        job.ActualAmount = request.JobRequest.ActualAmount;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job);
        await _jobRepository.SaveChangesAsync(cancellationToken);

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
            TotalCost = job.Expenses.Sum(e => e.Amount),
            ProfitMargin = job.ActualAmount.HasValue && job.Expenses.Any()
                ? job.ActualAmount.Value - job.Expenses.Sum(e => e.Amount)
                : null,
            CreatedAt = job.CreatedAt,
            UpdatedAt = job.UpdatedAt
        };
    }
}
