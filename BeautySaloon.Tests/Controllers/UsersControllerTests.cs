using System.Security.Claims;
using BeautySaloon_API.Controllers;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BeautySaloon.Tests.Controllers;

public class UsersControllerTests
{
    private readonly IUserService _userService;
    private readonly UsersController _sut;

    public UsersControllerTests()
    {
        _userService = Substitute.For<IUserService>();
        _sut = new UsersController(_userService);
    }

    private static ClaimsPrincipal MakeUser(int id) =>
        new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }));

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "A", Email = "a@a.com", Role = "Admin" },
            new() { Id = 2, FirstName = "Bob",   LastName = "B", Email = "b@b.com", Role = "Client" }
        };
        _userService.GetAll(Arg.Any<CancellationToken>()).Returns(users);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(2, ((IEnumerable<UserDto>)ok.Value!).Count());
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsOk()
    {
        // Arrange
        var dto = new UserDto { Id = 1, FirstName = "Alice", Email = "a@a.com", Role = "Client" };
        _userService.GetById(1, Arg.Any<CancellationToken>()).Returns(dto);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, ((UserDto)ok.Value!).Id);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _userService.GetById(99, Arg.Any<CancellationToken>()).Returns((UserDto?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetMe_WhenClaimPresent_ReturnsOk()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = MakeUser(5) }
        };
        var dto = new UserDto { Id = 5, FirstName = "Me", Email = "me@me.com", Role = "Client" };
        _userService.GetById(5, Arg.Any<CancellationToken>()).Returns(dto);

        // Act
        var result = await _sut.GetMe();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(5, ((UserDto)ok.Value!).Id);
    }

    [Fact]
    public async Task GetMe_WhenNoUserClaim_ReturnsUnauthorized()
    {
        // Arrange — empty identity, no claims
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await _sut.GetMe();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Update_WhenFound_ReturnsOk()
    {
        // Arrange
        var dto     = new UpdateUserDto { FirstName = "New", LastName = "Name", Email = "new@new.com", Role = "Client" };
        var updated = new UserDto { Id = 1, FirstName = "New", LastName = "Name", Email = "new@new.com", Role = "Client" };
        _userService.Update(1, dto, Arg.Any<CancellationToken>()).Returns(updated);

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("New", ((UserDto)ok.Value!).FirstName);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var dto = new UpdateUserDto { Email = "x@x.com" };
        _userService.Update(99, dto, Arg.Any<CancellationToken>()).Returns((UserDto?)null);

        // Act
        var result = await _sut.Update(99, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_WhenEmailConflict_ReturnsConflict()
    {
        // Arrange
        var dto = new UpdateUserDto { Email = "taken@x.com" };
        _userService.Update(1, dto, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Email is already in use."));

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenFound_ReturnsNoContent()
    {
        // Arrange
        _userService.Delete(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _userService.Delete(99, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Delete(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
