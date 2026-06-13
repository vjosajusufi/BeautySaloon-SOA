using AutoMapper;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Helpers;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services;
using NSubstitute;
using Xunit;

namespace BeautySaloon.Tests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _repository;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        var mapper  = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _sut        = new UserService(_repository, mapper);
    }

    private static User MakeUser(int id, string email = "user@example.com") =>
        new() { Id = id, FirstName = "Jane", LastName = "Doe", Email = email, PasswordHash = "hash", Role = "Client" };

    [Fact]
    public async Task GetAll_ReturnsMappedDtos()
    {
        // Arrange
        _repository.GetAll(Arg.Any<CancellationToken>()).Returns(new[] { MakeUser(1), MakeUser(2, "b@b.com") });

        // Act
        var result = (await _sut.GetAll()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, u => Assert.False(string.IsNullOrEmpty(u.Email)));
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsMappedDto()
    {
        // Arrange
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(MakeUser(1));

        // Act
        var result = await _sut.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmail_WhenExists_ReturnsMappedDto()
    {
        // Arrange
        _repository.GetByEmail("user@example.com", Arg.Any<CancellationToken>()).Returns(MakeUser(3));

        // Act
        var result = await _sut.GetByEmail("user@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user@example.com", result!.Email);
    }

    [Fact]
    public async Task GetByEmail_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetByEmail("nobody@x.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.GetByEmail("nobody@x.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Update_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetById(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.Update(99, new UpdateUserDto { Email = "x@x.com" });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Update_WhenSameEmail_ReturnsMappedDto()
    {
        // Arrange — email unchanged, no uniqueness check needed
        var existing = MakeUser(1, "same@example.com");
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.Update(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<User>());

        var dto = new UpdateUserDto { FirstName = "Updated", LastName = "Name", Email = "same@example.com", Role = "Client" };

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result!.FirstName);
    }

    [Fact]
    public async Task Update_WhenNewEmailAlreadyTaken_ThrowsInvalidOperationException()
    {
        // Arrange
        var existing = MakeUser(1, "old@example.com");
        var other    = MakeUser(2, "taken@example.com");
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.GetByEmail("taken@example.com", Arg.Any<CancellationToken>()).Returns(other);

        var dto = new UpdateUserDto { FirstName = "J", LastName = "D", Email = "taken@example.com", Role = "Client" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Update(1, dto));
    }

    [Fact]
    public async Task Update_WhenNewEmailFree_ReturnsMappedDto()
    {
        // Arrange
        var existing = MakeUser(1, "old@example.com");
        _repository.GetById(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.GetByEmail("new@example.com", Arg.Any<CancellationToken>()).Returns((User?)null);
        _repository.Update(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<User>());

        var dto = new UpdateUserDto { FirstName = "Jane", LastName = "Doe", Email = "new@example.com", Role = "Client" };

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result!.Email);
    }

    [Fact]
    public async Task Delete_Delegates_ToRepository()
    {
        // Arrange
        _repository.Delete(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.True(result);
        await _repository.Received(1).Delete(1, Arg.Any<CancellationToken>());
    }
}
