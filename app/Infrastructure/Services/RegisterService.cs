using Core.Interfaces;
using Core.Data.Entity;
using Core.Repository;
using Core.Data.DTOs;

namespace Infrastructure.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public RegisterService(IUserRepository userRepository, IPasswordHasher passwordHasher, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
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
    }
}
