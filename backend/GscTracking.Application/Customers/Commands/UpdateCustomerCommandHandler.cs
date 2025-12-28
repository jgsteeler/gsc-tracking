using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Customers.Commands;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (customer == null)
        {
            return null;
        }

        customer.Name = request.CustomerRequest.Name;
        customer.Email = request.CustomerRequest.Email;
        customer.Phone = request.CustomerRequest.Phone;
        customer.Address = request.CustomerRequest.Address;
        customer.Notes = request.CustomerRequest.Notes;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Notes = customer.Notes,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
