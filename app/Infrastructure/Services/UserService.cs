using Core.Interfaces;
using Core.Models;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.DTOs;
using Org.BouncyCastle.Asn1.Cmp;
using Core.Repository;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository repository;
    public UserService(IUserRepository _repository)
    {
        this.repository = _repository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await repository.GetAllUserAsync();
    }
}