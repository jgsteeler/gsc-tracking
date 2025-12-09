using Microsoft.EntityFrameworkCore;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;

namespace GscTracking.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(string? searchTerm = null)
    {
        var query = _context.Customer.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                c.Email.ToLower().Contains(searchTerm) ||
                c.Phone.Contains(searchTerm) ||
                c.Address.ToLower().Contains(searchTerm));
        }

        var customers = await query
            .OrderBy(c => c.Name)
            .ToListAsync();

        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            Notes = c.Notes,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _context.Customer
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (customer == null)
        {
            return null;
        }

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

    public async Task<CustomerDto> CreateCustomerAsync(CustomerRequestDto customerRequest)
    {
        var customer = new Customer
        {
            Name = customerRequest.Name,
            Email = customerRequest.Email,
            Phone = customerRequest.Phone,
            Address = customerRequest.Address,
            Notes = customerRequest.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Customer.Add(customer);
        await _context.SaveChangesAsync();

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

    public async Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerRequestDto customerRequest)
    {
        var customer = await _context.Customer
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (customer == null)
        {
            return null;
        }

        customer.Name = customerRequest.Name;
        customer.Email = customerRequest.Email;
        customer.Phone = customerRequest.Phone;
        customer.Address = customerRequest.Address;
        customer.Notes = customerRequest.Notes;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

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

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customer
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (customer == null)
        {
            return false;
        }

        // Soft delete
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}
