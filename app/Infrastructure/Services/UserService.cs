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
    private readonly IMatchRepository _matchRepository;
    public UserService(IConfiguration config, IUserRepository userRepository, IPasswordHasher passwordHasher, ISessionRepository sessionRepository, IEmailService emailService, IMatchRepository matchRepository)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _sessionRepository = sessionRepository;
        _emailService = emailService;
        _matchRepository = matchRepository;
    }

    public async Task<bool> IsUserOnlineAsync(int userId)
    {
        return await _sessionRepository.HasActiveSessionAsync(userId);
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
            Firstname = registerDto.Firstname ?? string.Empty,
            Lastname = registerDto.Lastname ?? string.Empty,
            Username = registerDto.Username ?? string.Empty,
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
    
    public async Task DeleteSessionsByUserIdAsync(int userId)
    {
        await _sessionRepository.DeleteSessionsByUserIdAsync(userId);
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Age = user.Age,
            NotifIsActive = user.NotifIsActive,
            Username = user.Username,
            Lastname = user.Lastname,
            Firstname = user.Firstname,
            Genre = user.Genre,
            Tag = user.Tag,
            SexualPreferences = user.SexualPreferences,
            Biography = user.Biography,
            Latitude = user.Latitude,
            Longitude = user.Longitude,
            PopularityScore = user.PopularityScore,
            ProfileComplete = user.ProfileComplete,
            LocalisationIsActive = user.LocalisationIsActive
        };
    }

    public async Task<bool> UpdateUserProfileAsync(UserProfileDto profileDto)
    {
        var user = await _userRepository.GetByIdAsync(profileDto.Id);
        if (user == null)
            return false;

        // Mise à jour des champs autorisés
        user.Firstname = profileDto.Firstname;
        user.Lastname = profileDto.Lastname;
        user.Username = profileDto.Username;
        user.Email = profileDto.Email;
        user.Age = profileDto.Age;
        user.NotifIsActive = profileDto.NotifIsActive;
        user.Genre = profileDto.Genre;
        user.Tag = profileDto.Tag;
        user.SexualPreferences = profileDto.SexualPreferences;
        user.Biography = profileDto.Biography;
        user.Latitude = profileDto.Latitude;
        user.Longitude = profileDto.Longitude;
        user.LocalisationIsActive = profileDto.LocalisationIsActive;
        
        //TODO faire le check des tags pour la variable profilecomplete
        bool isProfileComplete = !string.IsNullOrWhiteSpace(user.Firstname) &&
                             !string.IsNullOrWhiteSpace(user.Lastname) &&
                             !string.IsNullOrWhiteSpace(user.Username) &&
                             !string.IsNullOrWhiteSpace(user.Email) &&
                             user.Genre.HasValue &&
                             !string.IsNullOrWhiteSpace(user.Biography);

        user.ProfileComplete = isProfileComplete;

        await _userRepository.UpdateUserAsync(user);

        return true;
    }

    /** 
    * Met à jour le mot de passe de l'utilisateur.
    * @param userId L'ID de l'utilisateur.
    * @param newPassword Le nouveau mot de passe en clair.
    * @return true si la mise à jour a réussi, false sinon.
    */
    public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        // Hashe le nouveau mot de passe et met à jour l'utilisateur
        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _userRepository.Update(user);
        return true;
    }

    /**
    * Met à jour la localisation de l'utilisateur.
    * @param userId L'ID de l'utilisateur.
    * @param latitude La latitude de la position.
    * @param longitude La longitude de la position.
    * @param localisationIsActive Indique si la géolocalisation est active.
    */
    public async Task UpdateLocationAsync(int userId, double latitude, double longitude, bool localisationIsActive = false)
    {
        // Récupère l'utilisateur par son ID
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return; // Arrête si l'utilisateur n'existe pas

        // Met à jour la localisation et le statut de géolocalisation
        user.Latitude = latitude;
        user.Longitude = longitude;
        user.LocalisationIsActive = localisationIsActive;

        // Persiste les modifications en base de données
        await _userRepository.UpdateUserAsync(user);
    }
    
    public Task<DateTime?> GetLastActivityAsync(int userId)
        => _userRepository.GetLastActivityAsync(userId);

    public async Task<bool> HasMatchWithUserAsync(int currentUserId, int otherUserId)
    {
        return await _matchRepository.HasMatchAsync(currentUserId, otherUserId);
    }
}