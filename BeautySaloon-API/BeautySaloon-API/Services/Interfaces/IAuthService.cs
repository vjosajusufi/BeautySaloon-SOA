using BeautySaloon_API.DTOs;

namespace BeautySaloon_API.Services.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto, CancellationToken ct = default);
    Task<string> Login(LoginDto dto, CancellationToken ct = default);
}
