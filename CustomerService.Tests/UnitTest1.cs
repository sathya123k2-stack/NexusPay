using CustomerService.Models;

namespace CustomerService.Tests;

public class CustomerTests
{
    [Fact]
    public void Customer_ShouldStorePropertiesCorrectly()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "Sathya Reddy",
            Email = "sathya@example.com",
            Phone = "9876543210"
        };

        // Assert
        Assert.Equal(1, customer.Id);
        Assert.Equal("Sathya Reddy", customer.Name);
        Assert.Equal("sathya@example.com", customer.Email);
        Assert.Equal("9876543210", customer.Phone);
    }
}