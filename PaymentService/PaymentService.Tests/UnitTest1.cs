using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Controllers;
using PaymentService.Data;
using PaymentService.Models;
using Xunit;

namespace PaymentService.Tests;

public class PaymentServiceTests
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
    public async Task GetAll_ReturnsAllPayments()
    {
        await using var context = CreateContext();

        context.Payments.AddRange(
            new Payment
            {
                AccountId = 1,
                Amount = 1500,
                Currency = "INR",
                PaymentMethod = "UPI",
                PaymentStatus = "Completed",
                Description = "Test payment"
            },
            new Payment
            {
                AccountId = 1,
                Amount = 2500,
                Currency = "INR",
                PaymentMethod = "Card",
                PaymentStatus = "Pending",
                Description = "Second payment"
            });

        await context.SaveChangesAsync();

        var service = new PaymentService.Services.PaymentService(context);

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsPayment_WhenPaymentExists()
    {
        await using var context = CreateContext();

        var payment = new Payment
        {
            AccountId = 1,
            Amount = 1500,
            Currency = "INR",
            PaymentMethod = "UPI",
            PaymentStatus = "Completed",
            Description = "Test payment"
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        var service = new PaymentService.Services.PaymentService(context);

        var result = await service.GetById(payment.Id);

        Assert.NotNull(result);
        Assert.Equal(1500, result.Amount);
        Assert.Equal("Completed", result.PaymentStatus);
    }

    [Fact]
    public async Task Add_AddsPaymentToDatabase()
    {
        await using var context = CreateContext();

        var service = new PaymentService.Services.PaymentService(context);

        var payment = new Payment
        {
            AccountId = 1,
            Amount = 3000,
            Currency = "INR",
            PaymentMethod = "UPI",
            PaymentStatus = "Pending",
            Description = "New payment"
        };

        var result = await service.Add(payment);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Payments.CountAsync());
    }

    // =========================
    // CONTROLLER TESTS
    // =========================

    [Fact]
    public async Task Controller_GetAll_ReturnsOkWithPayments()
    {
        await using var context = CreateContext();

        context.Payments.Add(new Payment
        {
            AccountId = 1,
            Amount = 1500,
            Currency = "INR",
            PaymentMethod = "UPI",
            PaymentStatus = "Completed",
            Description = "Test payment"
        });

        await context.SaveChangesAsync();

        var service = new PaymentService.Services.PaymentService(context);
        var controller = new PaymentController(service);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var payments = Assert.IsType<List<Payment>>(okResult.Value);

        Assert.Single(payments);
        Assert.Equal(1500, payments[0].Amount);
        Assert.Equal("Completed", payments[0].PaymentStatus);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsOk_WhenPaymentExists()
    {
        await using var context = CreateContext();

        var payment = new Payment
        {
            AccountId = 1,
            Amount = 1500,
            Currency = "INR",
            PaymentMethod = "UPI",
            PaymentStatus = "Completed",
            Description = "Test payment"
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        var service = new PaymentService.Services.PaymentService(context);
        var controller = new PaymentController(service);

        var result = await controller.GetById(payment.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPayment = Assert.IsType<Payment>(okResult.Value);

        Assert.Equal(payment.Id, returnedPayment.Id);
        Assert.Equal(1500, returnedPayment.Amount);
        Assert.Equal("Completed", returnedPayment.PaymentStatus);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsNotFound_WhenPaymentDoesNotExist()
    {
        await using var context = CreateContext();

        var service = new PaymentService.Services.PaymentService(context);
        var controller = new PaymentController(service);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Controller_Add_ReturnsCreatedAtAction_WhenPaymentIsAdded()
    {
        await using var context = CreateContext();

        var service = new PaymentService.Services.PaymentService(context);
        var controller = new PaymentController(service);

        var payment = new Payment
        {
            AccountId = 1,
            Amount = 3000,
            Currency = "INR",
            PaymentMethod = "UPI",
            PaymentStatus = "Pending",
            Description = "New payment"
        };

        var result = await controller.Add(payment);

        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(
            nameof(PaymentController.GetById),
            createdResult.ActionName);

        Assert.NotNull(createdResult.Value);

        var createdPayment =
            Assert.IsType<Payment>(createdResult.Value);

        Assert.NotEqual(0, createdPayment.Id);
        Assert.Equal(3000, createdPayment.Amount);
        Assert.Equal("Pending", createdPayment.PaymentStatus);
    }
}