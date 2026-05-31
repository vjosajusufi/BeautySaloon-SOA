using AutoMapper;
using BeautySaloon_API.DTOs;
using BeautySaloon_API.Repositories.Interfaces;
using BeautySaloon_API.Services.Interfaces;

namespace BeautySaloon_API.Services;

public class UserService(IUserRepository repository, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAll(CancellationToken ct = default)
    {
        var users = await repository.GetAll(ct);
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetById(int id, CancellationToken ct = default)
    {
        var user = await repository.GetById(id, ct);
        return user is null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByEmail(string email, CancellationToken ct = default)
    {
        var user = await repository.GetByEmail(email, ct);
        return user is null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> Update(int id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var existing = await repository.GetById(id, ct);
        if (existing is null) return null;

        if (!string.Equals(existing.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await repository.GetByEmail(dto.Email, ct);
            if (emailTaken is not null)
                throw new InvalidOperationException("Email is already in use.");
        }

        mapper.Map(dto, existing);
        return mapper.Map<UserDto>(await repository.Update(existing, ct));
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default) =>
        await repository.Delete(id, ct);
}
