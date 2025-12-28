using GscTracking.Core.Entities;

namespace GscTracking.Core.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
