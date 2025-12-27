using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.JobUpdates.Queries;

public class GetJobUpdatesQueryHandler : IRequestHandler<GetJobUpdatesQuery, IEnumerable<JobUpdateDto>>
{
    private readonly IJobUpdateRepository _jobUpdateRepository;

    public GetJobUpdatesQueryHandler(IJobUpdateRepository jobUpdateRepository)
    {
        _jobUpdateRepository = jobUpdateRepository;
    }

    public async Task<IEnumerable<JobUpdateDto>> Handle(GetJobUpdatesQuery request, CancellationToken cancellationToken)
    {
        var updates = await _jobUpdateRepository.GetUpdatesByJobIdAsync(request.JobId);

        return updates.Select(u => new JobUpdateDto
        {
            Id = u.Id,
            JobId = u.JobId,
            UpdateText = u.UpdateText,
            CreatedAt = u.CreatedAt
        });
    }
}
