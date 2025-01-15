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
                PasswordHash = passwordHash
            };
            
            await _userRepository.Add(user);
            // Envoyer l'e-mail de confirmation
            var subject = "Confirmation de votre inscription";
            var body = $"<p>Bonjour {user.Username},</p><p>Merci de vous être inscrit. Veuillez confirmer votre compte en cliquant sur le lien suivant :</p><p><a href='https://votre-site.com/confirmation?email={user.Email}'>Confirmer mon compte</a></p>";
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return true;
        }
    }
}
