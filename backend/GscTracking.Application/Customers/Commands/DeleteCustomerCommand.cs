using MediatR;

namespace GscTracking.Application.Customers.Commands;

public record DeleteCustomerCommand(int Id) : IRequest<bool>;
