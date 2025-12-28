using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.JobUpdates.Commands;

public class DeleteJobUpdateCommandHandler : IRequestHandler<DeleteJobUpdateCommand, bool>
{
    private readonly IJobUpdateRepository _jobUpdateRepository;

    public DeleteJobUpdateCommandHandler(IJobUpdateRepository jobUpdateRepository)
    {
        _jobUpdateRepository = jobUpdateRepository;
    }

    public async Task<bool> Handle(DeleteJobUpdateCommand request, CancellationToken cancellationToken)
    {
        return await _jobUpdateRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
