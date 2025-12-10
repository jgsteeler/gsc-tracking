using GscTracking.Api.DTOs;

namespace GscTracking.Api.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(string? searchTerm = null);
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CustomerRequestDto customerRequest);
    Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerRequestDto customerRequest);
    Task<bool> DeleteCustomerAsync(int id);
}
