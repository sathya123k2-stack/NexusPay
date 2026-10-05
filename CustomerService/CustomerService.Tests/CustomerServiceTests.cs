using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerService.Tests;

public class CustomerServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllCustomers()
    {
        // Arrange
        await using var context = CreateContext();

        context.Customers.AddRange(
            new Customer
            {
                Name = "Test Customer 1",
                Email = "test1@example.com",
                Phone = "1111111111"
            },
            new Customer
            {
                Name = "Test Customer 2",
                Email = "test2@example.com",
                Phone = "2222222222"
            });

        await context.SaveChangesAsync();

        var service = new CustomerService.Services.CustomerService(context);

        // Act
        var result = await service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsCustomer_WhenCustomerExists()
    {
        // Arrange
        await using var context = CreateContext();

        var customer = new Customer
        {
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "1234567890"
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var service = new CustomerService.Services.CustomerService(context);

        // Act
        var result = await service.GetById(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Customer", result.Name);
    }

    [Fact]
    public async Task Add_AddsCustomerToDatabase()
    {
        // Arrange
        await using var context = CreateContext();

        var service = new CustomerService.Services.CustomerService(context);

        var customer = new Customer
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "9999999999"
        };

        // Act
        var result = await service.Add(customer);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Customers.CountAsync());
    }
}