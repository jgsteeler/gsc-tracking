using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Jobs.Commands;

public record UpdateJobCommand(int Id, JobRequestDto JobRequest) : IRequest<JobDto?>;
