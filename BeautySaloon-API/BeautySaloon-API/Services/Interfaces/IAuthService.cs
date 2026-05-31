using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);
    Task<string> Login(LoginDto dto);
}
