using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAll() =>
        await context.Users.ToListAsync();

    public async Task<User?> GetById(int id) =>
        await context.Users.FindAsync(id);

    public async Task<User?> GetByEmail(string email) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> Update(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> Delete(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null) return false;
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }
}
