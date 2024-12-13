using Core.Interfaces;
using Core.Models;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.DTOs;
using Org.BouncyCastle.Asn1.Cmp;

namespace Core.Repository
{
    public interface IUserRepository
    {
        Task<List<UserDto>>GetAllUserAsync();
    }
}