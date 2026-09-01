using CustomerService.Data;
using CustomerService.Models;

namespace CustomerService.Services;

public class CustomerServiceManager
{
    private readonly ApplicationDbContext _context;

    public CustomerServiceManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Customer> GetCustomers()
    {
        return _context.Customers.ToList();
    }

    public Customer CreateCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();

        return customer;
    }
}
