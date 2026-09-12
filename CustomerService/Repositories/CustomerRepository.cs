using CustomerService.Models;

namespace CustomerService.Repositories;

public class CustomerRepository
{
    public Customer GetCustomer()
    {
        return new Customer
        {
            Id = 1,
            Name = "Sathya",
            Email = "sathya@example.com",
            Phone = "9876543210"
        };
    }
}