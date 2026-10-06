using Microsoft.EntityFrameworkCore;
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
}