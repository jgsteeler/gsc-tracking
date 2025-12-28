using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Jobs.Commands;

public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand, bool>
{
    private readonly IJobRepository _jobRepository;

    public DeleteJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<bool> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (job == null)
        {
            return false;
        }

        // Soft delete
        job.IsDeleted = true;
        job.DeletedAt = DateTime.UtcNow;
        await _jobRepository.UpdateAsync(job);
        await _jobRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
