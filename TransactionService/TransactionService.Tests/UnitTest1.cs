using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Models;
using Xunit;

namespace TransactionService.Tests;

public class TransactionServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllTransactions()
    {
        await using var context = CreateContext();

        context.Transactions.AddRange(
            new Transaction
            {
                AccountId = 1,
                TransactionType = "Deposit",
                Amount = 5000,
                Currency = "INR",
                Description = "Test deposit"
            },
            new Transaction
            {
                AccountId = 1,
                TransactionType = "Withdrawal",
                Amount = 1000,
                Currency = "INR",
                Description = "Test withdrawal"
            });

        await context.SaveChangesAsync();

        var service = new TransactionService.Services.TransactionService(context);

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsTransaction_WhenTransactionExists()
    {
        await using var context = CreateContext();

        var transaction = new Transaction
        {
            AccountId = 1,
            TransactionType = "Deposit",
            Amount = 5000,
            Currency = "INR",
            Description = "Initial deposit"
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        var service = new TransactionService.Services.TransactionService(context);

        var result = await service.GetById(transaction.Id);

        Assert.NotNull(result);
        Assert.Equal(5000, result.Amount);
        Assert.Equal("Deposit", result.TransactionType);
    }

    [Fact]
    public async Task Add_AddsTransactionToDatabase()
    {
        await using var context = CreateContext();

        var service = new TransactionService.Services.TransactionService(context);

        var transaction = new Transaction
        {
            AccountId = 1,
            TransactionType = "Deposit",
            Amount = 2500,
            Currency = "INR",
            Description = "New transaction"
        };

        var result = await service.Add(transaction);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Transactions.CountAsync());
    }
}