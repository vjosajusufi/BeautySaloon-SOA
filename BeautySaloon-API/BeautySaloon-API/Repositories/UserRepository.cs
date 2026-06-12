using BeautySaloon_API.Data;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautySaloon_API.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAll(CancellationToken ct = default) =>
        await context.Users.AsNoTracking().ToListAsync(ct);

    public async Task<User?> GetById(int id, CancellationToken ct = default) =>
        await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmail(string email, CancellationToken ct = default) =>
        await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User> Update(User user, CancellationToken ct = default)
    {
        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default)
    {
        var user = await context.Users.FindAsync(new object[] { id }, ct);
        if (user is null) return false;
        context.Users.Remove(user);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
