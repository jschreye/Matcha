using Core.Interfaces.Services;
using Core.Data.Entity;
using Core.Interfaces.Repository;
using Core.Data.DTOs;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly string _connectionString;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionRepository _sessionRepository;
    private readonly IEmailService _emailService;
    public UserService(IConfiguration config, IUserRepository userRepository, IPasswordHasher passwordHasher, ISessionRepository sessionRepository, IEmailService emailService)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _sessionRepository = sessionRepository;
        _emailService = emailService;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUserAsync();
    }

    public async Task<bool> ValidateUser(string username, string password)
    {
        // Utilise le UserRepository pour récupérer le haché
        var dbPasswordHash = await _userRepository.GetPasswordHashByUsernameAsync(username);
        if (string.IsNullOrEmpty(dbPasswordHash))
        {
            return false;
        }

        // Vérifie le mot de passe
        return _passwordHasher.VerifyPassword(password, dbPasswordHash);
    }

    public async Task<bool> RegisterUser(RegisterDto registerDto)
    {
        // Vérifications nulles pour les propriétés critiques
        if (string.IsNullOrWhiteSpace(registerDto.Email))
            throw new ArgumentException("L'e-mail est requis.", nameof(registerDto.Email));

        if (string.IsNullOrWhiteSpace(registerDto.Password))
            throw new ArgumentException("Le mot de passe est requis.", nameof(registerDto.Password));

        var existingUser = await _userRepository.GetByEmail(registerDto.Email);
        if (existingUser != null)
            throw new Exception("Cet e-mail est déjà utilisé.");

        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);
        var user = new User
        {
            Firstname = registerDto.Firstname,
            Lastname = registerDto.Lastname,
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            ActivationToken = Guid.NewGuid().ToString()  // Génère un token unique
        };

        await _userRepository.Add(user);

        // Construire le lien de confirmation avec l'email et le token
        var activationLink = $"http://localhost:80/auth/confirmation?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(user.ActivationToken)}";

        var subject = "Confirmation de votre inscription";
        var body = $"<p>Bonjour {user.Username},</p>" +
                $"<p>Merci de vous être inscrit. Veuillez confirmer votre compte en cliquant sur le lien suivant :</p>" +
                $"<p><a href='{activationLink}'>Confirmer mon compte</a></p>";

        await _emailService.SendEmailAsync(user.Email, subject, body);

        return true;
    }
    public async Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt)
    {
        await _sessionRepository.CreateSessionAsync(userId, sessionToken, expiresAt);
    }

    public async Task<bool> ValidateSessionAsync(string sessionToken)
    {
        return await _sessionRepository.ValidateSessionAsync(sessionToken);
    }

    public async Task DeleteSessionAsync(string sessionToken)
    {
        await _sessionRepository.DeleteSessionAsync(sessionToken);
    }
}