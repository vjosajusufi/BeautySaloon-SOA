using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAll();
    Task<UserDto?> GetById(int id);
    Task<UserDto?> GetByEmail(string email);
    Task<UserDto?> Update(int id, UpdateUserDto dto);
    Task<bool> Delete(int id);
}
