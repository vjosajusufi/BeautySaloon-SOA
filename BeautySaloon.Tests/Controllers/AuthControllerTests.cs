using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class AuthControllerTests
{
    private readonly IAuthService _authService;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _authService = Substitute.For<IAuthService>();
        _sut = new AuthController(_authService);
    }

    [Fact]
    public async Task Register_WhenSuccessful_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new RegisterDto { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Password = "Pass1!" };
        _authService.Register(dto, Arg.Any<CancellationToken>()).Returns("jwt.token.here");

        // Act
        var result = await _sut.Register(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = ok.Value!;
        Assert.NotNull(value.GetType().GetProperty("token")?.GetValue(value));
    }

    [Fact]
    public async Task Register_WhenDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var dto = new RegisterDto { Email = "taken@example.com", Password = "Pass1!" };
        _authService.Register(dto, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Email is already registered."));

        // Act
        var result = await _sut.Register(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    [Fact]
    public async Task Login_WhenSuccessful_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new LoginDto { Email = "jane@example.com", Password = "Pass1!" };
        _authService.Login(dto, Arg.Any<CancellationToken>()).Returns("jwt.token.here");

        // Act
        var result = await _sut.Login(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Login_WhenWrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new LoginDto { Email = "jane@example.com", Password = "WrongPass!" };
        _authService.Login(dto, Arg.Any<CancellationToken>())
            .Throws(new UnauthorizedAccessException("Invalid email or password."));

        // Act
        var result = await _sut.Login(dto);

        // Assert
        var unauth = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauth.Value);
    }
}
