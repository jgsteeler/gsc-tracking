using GscTracking.Application.DTOs;
using GscTracking.Core.Entities;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Customers.Commands;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.CustomerRequest.Name,
            Email = request.CustomerRequest.Email,
            Phone = request.CustomerRequest.Phone,
            Address = request.CustomerRequest.Address,
            Notes = request.CustomerRequest.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdCustomer = await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return new CustomerDto
        {
            Id = createdCustomer.Id,
            Name = createdCustomer.Name,
            Email = createdCustomer.Email,
            Phone = createdCustomer.Phone,
            Address = createdCustomer.Address,
            Notes = createdCustomer.Notes,
            CreatedAt = createdCustomer.CreatedAt,
            UpdatedAt = createdCustomer.UpdatedAt
        };
    }
}
