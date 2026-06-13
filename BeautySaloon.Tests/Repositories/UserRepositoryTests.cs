using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeautySaloon.Tests.Repositories;

public class UserRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static User MakeUser(string email = "user@example.com") =>
        new() { FirstName = "Jane", LastName = "Doe", Email = email, PasswordHash = "hash", Role = "Client" };

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange
        await using var ctx = CreateContext();
        ctx.Users.AddRange(MakeUser("a@a.com"), MakeUser("b@b.com"));
        await ctx.SaveChangesAsync();
        var repo = new UserRepository(ctx);

        // Act
        var result = (await repo.GetAll()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsUser()
    {
        // Arrange
        await using var ctx = CreateContext();
        var user = MakeUser();
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        var repo = new UserRepository(ctx);

        // Act
        var result = await repo.GetById(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new UserRepository(ctx);

        // Act
        var result = await repo.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmail_WhenExists_ReturnsUser()
    {
        // Arrange
        await using var ctx = CreateContext();
        ctx.Users.Add(MakeUser("find@me.com"));
        await ctx.SaveChangesAsync();
        var repo = new UserRepository(ctx);

        // Act
        var result = await repo.GetByEmail("find@me.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("find@me.com", result!.Email);
    }

    [Fact]
    public async Task GetByEmail_WhenNotFound_ReturnsNull()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new UserRepository(ctx);

        // Act
        var result = await repo.GetByEmail("nobody@no.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Update_PersistsChanges()
    {
        // Arrange
        await using var ctx = CreateContext();
        var user = MakeUser();
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear(); // simulate AsNoTracking read

        var toUpdate = new User { Id = user.Id, FirstName = "Updated", LastName = "Name", Email = "updated@example.com", PasswordHash = "newhash", Role = "Admin" };
        var repo     = new UserRepository(ctx);

        // Act
        var result = await repo.Update(toUpdate);

        // Assert
        Assert.Equal("Updated", result.FirstName);
        var persisted = await ctx.Users.FindAsync(user.Id);
        Assert.Equal("Updated", persisted!.FirstName);
        Assert.Equal("Admin", persisted.Role);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesUserAndReturnsTrue()
    {
        // Arrange
        await using var ctx = CreateContext();
        var user = MakeUser();
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        var repo = new UserRepository(ctx);

        // Act
        var result = await repo.Delete(user.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await ctx.Users.CountAsync());
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        await using var ctx = CreateContext();
        var repo = new UserRepository(ctx);

        // Act
        var result = await repo.Delete(999);

        // Assert
        Assert.False(result);
    }
}
