using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Jobs.Queries;

public record GetJobsByCustomerIdQuery(int CustomerId) : IRequest<IEnumerable<JobDto>>;
