using BeautySaloon_API.Data;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BeautySaloon.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]        = "supersecretkey_that_is_long_enough_32chars!",
                ["Jwt:Issuer"]     = "TestIssuer",
                ["Jwt:Audience"]   = "TestAudience",
                ["Jwt:ExpiryDays"] = "7"
            })
            .Build();

        _sut = new AuthService(_context, config);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsJwtToken()
    {
        // Arrange
        var dto = new RegisterDto
        {
            FirstName = "Jane",
            LastName  = "Doe",
            Email     = "jane@example.com",
            Password  = "Password123!"
        };

        // Act
        var token = await _sut.Register(dto);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length); // JWT has 3 dot-separated parts
        var persisted = await _context.Users.SingleAsync(u => u.Email == dto.Email);
        Assert.Equal("Jane", persisted.FirstName);
        Assert.Equal("Client", persisted.Role);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange — seed an existing user
        _context.Users.Add(new User
        {
            FirstName    = "Existing",
            LastName     = "User",
            Email        = "taken@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
            Role         = "Client"
        });
        await _context.SaveChangesAsync();

        var dto = new RegisterDto
        {
            FirstName = "New",
            LastName  = "User",
            Email     = "taken@example.com",
            Password  = "NewPassword!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Register(dto));
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange — seed a user with a known password
        const string password = "Secret123!";
        _context.Users.Add(new User
        {
            FirstName    = "John",
            LastName     = "Smith",
            Email        = "john@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role         = "Client"
        });
        await _context.SaveChangesAsync();

        var dto = new LoginDto { Email = "john@example.com", Password = password };

        // Act
        var token = await _sut.Login(dto);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _context.Users.Add(new User
        {
            FirstName    = "Alice",
            LastName     = "Brown",
            Email        = "alice@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!"),
            Role         = "Client"
        });
        await _context.SaveChangesAsync();

        var dto = new LoginDto { Email = "alice@example.com", Password = "WrongPassword!" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Login(dto));
    }

    public void Dispose() => _context.Dispose();
}
