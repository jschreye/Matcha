using Core.Data.DTOs;

namespace Core.Interfaces
{
    public interface IRegisterService
    {
        Task<bool> RegisterUser(RegisterDto registerDto);
    }
}