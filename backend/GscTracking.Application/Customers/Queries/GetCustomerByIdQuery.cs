using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Customers.Queries;

public record GetCustomerByIdQuery(int Id) : IRequest<CustomerDto?>;
