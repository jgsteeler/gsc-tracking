using GscTracking.Application.DTOs;
using GscTracking.Core.Entities;
using GscTracking.Core.Enums;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Jobs.Commands;

public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, JobDto>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateJobCommandHandler(IJobRepository jobRepository, ICustomerRepository customerRepository)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
    }

    public async Task<JobDto> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        // Parse status from string
        if (!Enum.TryParse<JobStatus>(request.JobRequest.Status, true, out var status))
        {
            status = JobStatus.Quote; // Default status
        }

        var job = new Job
        {
            CustomerId = request.JobRequest.CustomerId,
            EquipmentType = request.JobRequest.EquipmentType,
            EquipmentModel = request.JobRequest.EquipmentModel,
            Description = request.JobRequest.Description,
            Status = status,
            DateReceived = request.JobRequest.DateReceived,
            DateCompleted = request.JobRequest.DateCompleted,
            EstimateAmount = request.JobRequest.EstimateAmount,
            ActualAmount = request.JobRequest.ActualAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdJob = await _jobRepository.AddAsync(job, cancellationToken);
        await _jobRepository.SaveChangesAsync(cancellationToken);

        // Fetch the customer name
        var customer = await _customerRepository.GetByIdAsync(createdJob.CustomerId, cancellationToken);

        return new JobDto
        {
            Id = createdJob.Id,
            CustomerId = createdJob.CustomerId,
            CustomerName = customer?.Name ?? string.Empty,
            EquipmentType = createdJob.EquipmentType,
            EquipmentModel = createdJob.EquipmentModel,
            Description = createdJob.Description,
            Status = createdJob.Status.ToString(),
            DateReceived = createdJob.DateReceived,
            DateCompleted = createdJob.DateCompleted,
            EstimateAmount = createdJob.EstimateAmount,
            ActualAmount = createdJob.ActualAmount,
            TotalCost = 0,
            ProfitMargin = null,
            CreatedAt = createdJob.CreatedAt,
            UpdatedAt = createdJob.UpdatedAt
        };
    }
}
