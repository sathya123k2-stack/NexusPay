using AccountService.Data;
using AccountService.Models;
using AccountService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AccountService.Tests;

public class AccountServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllAccounts()
    {
        await using var context = CreateContext();

        context.Accounts.AddRange(
            new Account
            {
                CustomerId = 1,
                AccountNumber = "100000000001",
                AccountType = "Savings",
                Balance = 10000.00m,
                Currency = "INR"
            },
            new Account
            {
                CustomerId = 2,
                AccountNumber = "100000000002",
                AccountType = "Current",
                Balance = 20000.00m,
                Currency = "INR"
            });

        await context.SaveChangesAsync();

        var service = new AccountService.Services.AccountService(context);

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsAccount_WhenAccountExists()
    {
        await using var context = CreateContext();

        var account = new Account
        {
            CustomerId = 1,
            AccountNumber = "100000000001",
            AccountType = "Savings",
            Balance = 25000.75m,
            Currency = "INR"
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var service = new AccountService.Services.AccountService(context);

        var result = await service.GetById(account.Id);

        Assert.NotNull(result);
        Assert.Equal("100000000001", result.AccountNumber);
        Assert.Equal(25000.75m, result.Balance);
    }

    [Fact]
    public async Task Add_AddsAccountToDatabase()
    {
        await using var context = CreateContext();

        var service = new AccountService.Services.AccountService(context);

        var account = new Account
        {
            CustomerId = 1,
            AccountNumber = "100000000003",
            AccountType = "Savings",
            Balance = 5000.00m,
            Currency = "INR"
        };

        var result = await service.Add(account);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Accounts.CountAsync());
    }
}
