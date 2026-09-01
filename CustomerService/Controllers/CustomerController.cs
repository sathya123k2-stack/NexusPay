using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Models;

namespace CustomerService.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerServiceManager _customerService;

    public CustomerController(CustomerServiceManager customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public List<Customer> GetCustomers()
    {
        return _customerService.GetCustomers();
    }
    [HttpPost]
    public Customer CreateCustomer(Customer customer)
    {
        return _customerService.CreateCustomer(customer);
    }
}