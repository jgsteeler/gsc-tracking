using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Customers.Commands;

public record CreateCustomerCommand(CustomerRequestDto CustomerRequest) : IRequest<CustomerDto>;
