using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.JobUpdates.Queries;

public class GetJobUpdateByIdQueryHandler : IRequestHandler<GetJobUpdateByIdQuery, JobUpdateDto?>
{
    private readonly IJobUpdateRepository _jobUpdateRepository;

    public GetJobUpdateByIdQueryHandler(IJobUpdateRepository jobUpdateRepository)
    {
        _jobUpdateRepository = jobUpdateRepository;
    }

    public async Task<JobUpdateDto?> Handle(GetJobUpdateByIdQuery request, CancellationToken cancellationToken)
    {
        var update = await _jobUpdateRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (update == null)
        {
            return null;
        }

        return new JobUpdateDto
        {
            Id = update.Id,
            JobId = update.JobId,
            UpdateText = update.UpdateText,
            CreatedAt = update.CreatedAt
        };
    }
}
