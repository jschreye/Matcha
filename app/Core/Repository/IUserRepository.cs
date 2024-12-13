using Core.Interfaces;
using Core.Models;
using System.Data;
using Core.DTOs;

namespace Core.Repository
{
    public interface IUserRepository
    {
        Task<List<UserDto>>GetAllUserAsync();
    }
}