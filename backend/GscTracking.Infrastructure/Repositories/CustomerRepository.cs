using Microsoft.EntityFrameworkCore;
using GscTracking.Core.Entities;
using GscTracking.Core.Interfaces;
using GscTracking.Infrastructure.Data;

namespace GscTracking.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
    {
        searchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                (c.Address != null && c.Address.ToLower().Contains(searchTerm)))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
