using AuthenticationService.Controllers;
using AuthenticationService.Data;
using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AuthenticationService.Tests;

public class AuthenticationServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "Test-Key-For-Unit-Testing-Only",
            ["Jwt:Issuer"] = "NexusPay",
            ["Jwt:Audience"] = "NexusPayUsers"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    // =========================
    // SERVICE TESTS
    // =========================

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        await using var context = CreateContext();

        context.Users.AddRange(
            new User
            {
                CustomerId = 1,
                Username = "testuser1",
                PasswordHash = "hash1",
                Role = "Customer"
            },
            new User
            {
                CustomerId = 2,
                Username = "testuser2",
                PasswordHash = "hash2",
                Role = "Customer"
            });

        await context.SaveChangesAsync();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ReturnsUser_WhenUserExists()
    {
        await using var context = CreateContext();

        var user = new User
        {
            CustomerId = 1,
            Username = "testuser",
            PasswordHash = "hash",
            Role = "Customer"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var result = await service.GetById(user.Id);

        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task Add_AddsUserToDatabase()
    {
        await using var context = CreateContext();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var user = new User
        {
            CustomerId = 1,
            Username = "newuser",
            PasswordHash = "hash",
            Role = "Customer"
        };

        var result = await service.Add(user);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Users.CountAsync());
    }

    // =========================
    // CONTROLLER TESTS
    // =========================

    [Fact]
    public async Task Controller_GetAll_ReturnsOkWithUsers()
    {
        await using var context = CreateContext();

        context.Users.Add(new User
        {
            CustomerId = 1,
            Username = "testuser",
            PasswordHash = "hash",
            Role = "Customer"
        });

        await context.SaveChangesAsync();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var controller = new UserController(service);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        Assert.Single(users);
        Assert.Equal("testuser", users[0].Username);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsOk_WhenUserExists()
    {
        await using var context = CreateContext();

        var user = new User
        {
            CustomerId = 1,
            Username = "testuser",
            PasswordHash = "hash",
            Role = "Customer"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var controller = new UserController(service);

        var result = await controller.GetById(user.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);

        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal("testuser", returnedUser.Username);
    }

    [Fact]
    public async Task Controller_GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        await using var context = CreateContext();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var controller = new UserController(service);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Controller_Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        await using var context = CreateContext();

        var service = new AuthenticationService.Services.AuthenticationService(
            context,
            CreateConfiguration());

        var controller = new UserController(service);

        var request = new LoginRequest
        {
            Username = "wronguser",
            Password = "WrongPassword"
        };

        var result = await controller.Login(request);

        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(result.Result);

        Assert.Equal(
            "Invalid username or password.",
            unauthorizedResult.Value);
    }
}