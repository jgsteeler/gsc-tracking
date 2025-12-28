using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Jobs.Queries;

public record GetJobByIdQuery(int Id) : IRequest<JobDto?>;
