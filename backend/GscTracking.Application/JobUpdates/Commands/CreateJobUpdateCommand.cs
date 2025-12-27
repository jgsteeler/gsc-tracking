using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.JobUpdates.Commands;

public record CreateJobUpdateCommand(int JobId, JobUpdateRequestDto UpdateRequest) : IRequest<JobUpdateDto>;
