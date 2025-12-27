using GscTracking.Application.DTOs;
using GscTracking.Core.Enums;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Jobs.Queries;

public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, IEnumerable<JobDto>>
{
    private readonly IJobRepository _jobRepository;

    public GetAllJobsQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<IEnumerable<JobDto>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<GscTracking.Core.Entities.Job> jobs;

        // Filter by status if provided
        if (!string.IsNullOrWhiteSpace(request.StatusFilter))
        {
            if (Enum.TryParse<JobStatus>(request.StatusFilter, true, out var status))
            {
                jobs = await _jobRepository.GetJobsByStatusAsync(status);
            }
            else
            {
                jobs = await _jobRepository.GetAllAsync();
            }
        }
        else
        {
            jobs = await _jobRepository.GetAllAsync();
        }

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            jobs = jobs.Where(j =>
                j.Customer.Name.ToLower().Contains(searchLower) ||
                j.EquipmentType.ToLower().Contains(searchLower) ||
                j.EquipmentModel.ToLower().Contains(searchLower) ||
                j.Description.ToLower().Contains(searchLower));
        }

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
