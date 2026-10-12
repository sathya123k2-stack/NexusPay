using AccountService.Controllers;
using AccountService.Data;
using AccountService.Models;
using AccountService.Services;
using Microsoft.AspNetCore.Mvc;
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

    // =========================
    // SERVICE TESTS
    // =========================

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

    // =========================
    // CONTROLLER TESTS
    // =========================

    [Fact]
    public async Task Controller_GetAll_ReturnsOkWithAccounts()
    {
        await using var context = CreateContext();

        context.Accounts.Add(new Account
        {
            CustomerId = 1,
            AccountNumber = "100000000001",
            AccountType = "Savings",
            Balance = 25000.75m,
            Currency = "INR"
        });

        await context.SaveChangesAsync();

        var service = new AccountService.Services.AccountService(context);
        var controller = new AccountController(service);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var accounts = Assert.IsType<List<Account>>(okResult.Value);

        Assert.Single(accounts);
        Assert.Equal("100000000001", accounts[0].AccountNumber);
        Assert.Equal(25000.75m, accounts[0].Balance);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsOk_WhenAccountExists()
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
        var controller = new AccountController(service);

        var result = await controller.GetById(account.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAccount = Assert.IsType<Account>(okResult.Value);

        Assert.Equal(account.Id, returnedAccount.Id);
        Assert.Equal("100000000001", returnedAccount.AccountNumber);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        await using var context = CreateContext();

        var service = new AccountService.Services.AccountService(context);
        var controller = new AccountController(service);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Controller_Add_ReturnsCreatedAtAction_WhenAccountIsAdded()
    {
        await using var context = CreateContext();

        var service = new AccountService.Services.AccountService(context);
        var controller = new AccountController(service);

        var account = new Account
        {
            CustomerId = 1,
            AccountNumber = "100000000004",
            AccountType = "Savings",
            Balance = 5000.00m,
            Currency = "INR"
        };

        var result = await controller.Add(account);

        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(
            nameof(AccountController.GetById),
            createdResult.ActionName);

        Assert.NotNull(createdResult.Value);

        var createdAccount =
            Assert.IsType<Account>(createdResult.Value);

        Assert.NotEqual(0, createdAccount.Id);
        Assert.Equal("100000000004", createdAccount.AccountNumber);
    }
}