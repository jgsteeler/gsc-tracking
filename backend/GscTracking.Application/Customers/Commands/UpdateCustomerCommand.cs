using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Customers.Commands;

public record UpdateCustomerCommand(int Id, CustomerRequestDto CustomerRequest) : IRequest<CustomerDto?>;
