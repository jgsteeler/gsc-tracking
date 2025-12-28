using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Customers.Commands;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (customer == null)
        {
            return false;
        }

        // Soft delete
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
