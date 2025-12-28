using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Jobs.Queries;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobDto?>
{
    private readonly IJobRepository _jobRepository;

    public GetJobByIdQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<JobDto?> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetJobWithDetailsAsync(request.Id, cancellationToken);
        
        if (job == null)
        {
            return null;
        }

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
