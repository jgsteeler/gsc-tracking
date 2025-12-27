using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Customers.Queries;

public record GetAllCustomersQuery(string? SearchTerm = null) : IRequest<IEnumerable<CustomerDto>>;
