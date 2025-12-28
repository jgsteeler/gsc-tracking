using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Jobs.Queries;

public class GetJobsByCustomerIdQueryHandler : IRequestHandler<GetJobsByCustomerIdQuery, IEnumerable<JobDto>>
{
    private readonly IJobRepository _jobRepository;

    public GetJobsByCustomerIdQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<IEnumerable<JobDto>> Handle(GetJobsByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _jobRepository.GetJobsByCustomerIdAsync(request.CustomerId, cancellationToken);

        return jobs.Select(j => new JobDto
        {
            Id = j.Id,
            CustomerId = j.CustomerId,
            CustomerName = j.Customer.Name,
            EquipmentType = j.EquipmentType,
            EquipmentModel = j.EquipmentModel,
            Description = j.Description,
            Status = j.Status.ToString(),
            DateReceived = j.DateReceived,
            DateCompleted = j.DateCompleted,
            EstimateAmount = j.EstimateAmount,
            ActualAmount = j.ActualAmount,
            TotalCost = j.Expenses.Sum(e => e.Amount),
            ProfitMargin = j.ActualAmount.HasValue && j.Expenses.Any()
                ? j.ActualAmount.Value - j.Expenses.Sum(e => e.Amount)
                : null,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt
        });
    }
}
