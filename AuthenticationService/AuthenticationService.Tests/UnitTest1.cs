using AuthenticationService.Data;
using AuthenticationService.Models;
using AuthenticationService.Services;
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
}
