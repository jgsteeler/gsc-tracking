using GscTracking.Application.DTOs;
using GscTracking.Core.Entities;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.JobUpdates.Commands;

public class CreateJobUpdateCommandHandler : IRequestHandler<CreateJobUpdateCommand, JobUpdateDto>
{
    private readonly IJobUpdateRepository _jobUpdateRepository;

    public CreateJobUpdateCommandHandler(IJobUpdateRepository jobUpdateRepository)
    {
        _jobUpdateRepository = jobUpdateRepository;
    }

    public async Task<JobUpdateDto> Handle(CreateJobUpdateCommand request, CancellationToken cancellationToken)
    {
        var jobUpdate = new JobUpdate
        {
            JobId = request.JobId,
            UpdateText = request.UpdateRequest.UpdateText,
            CreatedAt = DateTime.UtcNow
        };

        var createdUpdate = await _jobUpdateRepository.AddAsync(jobUpdate);
        await _jobUpdateRepository.SaveChangesAsync();

        return new JobUpdateDto
        {
            Id = createdUpdate.Id,
            JobId = createdUpdate.JobId,
            UpdateText = createdUpdate.UpdateText,
            CreatedAt = createdUpdate.CreatedAt
        };
    }
}
