using BeautySaloon_API.DTOs;
using BeautySaloon_API.Models;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;

namespace BeautySaloon_API.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAll()
    {
        var users = await repository.GetAll();
        return users.Select(ToDto);
    }

    public async Task<UserDto?> GetById(int id)
    {
        var user = await repository.GetById(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        var user = await repository.GetByEmail(email);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto?> Update(int id, UpdateUserDto dto)
    {
        var existing = await repository.GetById(id);
        if (existing is null) return null;

        if (!string.Equals(existing.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await repository.GetByEmail(dto.Email);
            if (emailTaken is not null)
                throw new InvalidOperationException("Email is already in use.");
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.Role = dto.Role;

        return ToDto(await repository.Update(existing));
    }

    public async Task<bool> Delete(int id) =>
        await repository.Delete(id);

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Role = u.Role
    };
}
