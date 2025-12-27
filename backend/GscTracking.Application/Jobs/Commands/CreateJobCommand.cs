using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Jobs.Commands;

public record CreateJobCommand(JobRequestDto JobRequest) : IRequest<JobDto>;
