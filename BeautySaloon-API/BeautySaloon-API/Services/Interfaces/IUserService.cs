using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAll(CancellationToken ct = default);
    Task<UserDto?> GetById(int id, CancellationToken ct = default);
    Task<UserDto?> GetByEmail(string email, CancellationToken ct = default);
    Task<UserDto?> Update(int id, UpdateUserDto dto, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
