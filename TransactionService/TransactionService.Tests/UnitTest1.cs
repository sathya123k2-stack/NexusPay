using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionService.Controllers;
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

    // =========================
    // SERVICE TESTS
    // =========================

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

    // =========================
    // CONTROLLER TESTS
    // =========================

    [Fact]
    public async Task Controller_GetAll_ReturnsOkWithTransactions()
    {
        await using var context = CreateContext();

        context.Transactions.Add(new Transaction
        {
            AccountId = 1,
            TransactionType = "Deposit",
            Amount = 5000,
            Currency = "INR",
            Description = "Test deposit"
        });

        await context.SaveChangesAsync();

        var service = new TransactionService.Services.TransactionService(context);
        var controller = new TransactionController(service);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactions = Assert.IsType<List<Transaction>>(okResult.Value);

        Assert.Single(transactions);
        Assert.Equal(5000, transactions[0].Amount);
        Assert.Equal("Deposit", transactions[0].TransactionType);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsOk_WhenTransactionExists()
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
        var controller = new TransactionController(service);

        var result = await controller.GetById(transaction.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTransaction = Assert.IsType<Transaction>(okResult.Value);

        Assert.Equal(transaction.Id, returnedTransaction.Id);
        Assert.Equal(5000, returnedTransaction.Amount);
        Assert.Equal("Deposit", returnedTransaction.TransactionType);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        await using var context = CreateContext();

        var service = new TransactionService.Services.TransactionService(context);
        var controller = new TransactionController(service);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Controller_Add_ReturnsCreatedAtAction_WhenTransactionIsAdded()
    {
        await using var context = CreateContext();

        var service = new TransactionService.Services.TransactionService(context);
        var controller = new TransactionController(service);

        var transaction = new Transaction
        {
            AccountId = 1,
            TransactionType = "Deposit",
            Amount = 2500,
            Currency = "INR",
            Description = "New transaction"
        };

        var result = await controller.Add(transaction);

        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(
            nameof(TransactionController.GetById),
            createdResult.ActionName);

        Assert.NotNull(createdResult.Value);

        var createdTransaction =
            Assert.IsType<Transaction>(createdResult.Value);

        Assert.NotEqual(0, createdTransaction.Id);
        Assert.Equal(2500, createdTransaction.Amount);
        Assert.Equal("Deposit", createdTransaction.TransactionType);
    }
}