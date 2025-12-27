using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.JobUpdates.Queries;

public record GetJobUpdatesQuery(int JobId) : IRequest<IEnumerable<JobUpdateDto>>;
