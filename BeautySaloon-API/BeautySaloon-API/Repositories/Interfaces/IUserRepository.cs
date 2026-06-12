using BeautySaloon_API.Models;

namespace BeautySaloon_API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll(CancellationToken ct = default);
    Task<User?> GetById(int id, CancellationToken ct = default);
    Task<User?> GetByEmail(string email, CancellationToken ct = default);
    Task<User> Update(User user, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
