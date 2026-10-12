using CustomerService.Controllers;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;
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

    // =========================
    // SERVICE TESTS
    // =========================

    [Fact]
    public async Task GetAll_ReturnsAllCustomers()
    {
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

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsCustomer_WhenCustomerExists()
    {
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

        var result = await service.GetById(customer.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Customer", result.Name);
    }

    [Fact]
    public async Task Add_AddsCustomerToDatabase()
    {
        await using var context = CreateContext();

        var service = new CustomerService.Services.CustomerService(context);

        var customer = new Customer
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "9999999999"
        };

        var result = await service.Add(customer);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Customers.CountAsync());
    }

    // =========================
    // CONTROLLER TESTS
    // =========================

    [Fact]
    public async Task Controller_GetAll_ReturnsOkWithCustomers()
    {
        await using var context = CreateContext();

        context.Customers.Add(new Customer
        {
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "1234567890"
        });

        await context.SaveChangesAsync();

        var service = new CustomerService.Services.CustomerService(context);
        var controller = new CustomerController(service);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var customers = Assert.IsType<List<Customer>>(okResult.Value);

        Assert.Single(customers);
        Assert.Equal("Test Customer", customers[0].Name);
        Assert.Equal("test@example.com", customers[0].Email);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsOk_WhenCustomerExists()
    {
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
        var controller = new CustomerController(service);

        var result = await controller.GetById(customer.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCustomer = Assert.IsType<Customer>(okResult.Value);

        Assert.Equal(customer.Id, returnedCustomer.Id);
        Assert.Equal("Test Customer", returnedCustomer.Name);
        Assert.Equal("test@example.com", returnedCustomer.Email);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        await using var context = CreateContext();

        var service = new CustomerService.Services.CustomerService(context);
        var controller = new CustomerController(service);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Controller_Add_ReturnsCreatedAtAction_WhenCustomerIsAdded()
    {
        await using var context = CreateContext();

        var service = new CustomerService.Services.CustomerService(context);
        var controller = new CustomerController(service);

        var customer = new Customer
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "9999999999"
        };

        var result = await controller.Add(customer);

        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(
            nameof(CustomerController.GetById),
            createdResult.ActionName);

        Assert.NotNull(createdResult.Value);

        var createdCustomer =
            Assert.IsType<Customer>(createdResult.Value);

        Assert.NotEqual(0, createdCustomer.Id);
        Assert.Equal("New Customer", createdCustomer.Name);
        Assert.Equal("new@example.com", createdCustomer.Email);
    }
}
