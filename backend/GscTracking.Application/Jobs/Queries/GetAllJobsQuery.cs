using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Jobs.Queries;

public record GetAllJobsQuery(string? SearchTerm = null, string? StatusFilter = null) : IRequest<IEnumerable<JobDto>>;
