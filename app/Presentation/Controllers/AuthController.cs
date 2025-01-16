using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Repository;

namespace Presentation.Controllers // Remplacez par votre espace de noms approprié
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passWordHasher;

        public AuthController(IUserService userService, IUserRepository userRepository, IEmailService emailService, IPasswordHasher passWordHasher)
        {
            _userService = userService;
            _userRepository = userRepository;
            _emailService = emailService;
            _passWordHasher = passWordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            if (await _userService.ValidateUser(username, password))
            {
                var user = await _userRepository.FindByUsernameAsync(username);
                Console.WriteLine(user.IsActive);
                if (user.IsActive != true)
                {
                    Console.WriteLine("Compte non validé");
                    return Unauthorized("Compte non validé");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // ✅ Ajout de logs pour confirmer que le code passe bien ici
                Console.WriteLine("Utilisateur validé. Tentative de création du cookie.");

                // ✅ Test sans options pour limiter les restrictions
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                Console.WriteLine("Cookie d'authentification créé.");

                return Ok(new { Message = "Login réussi" });
            }
            Console.WriteLine("Identifiants incorrects");
            return Unauthorized("Identifiants incorrects");

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Déconnecté");
        }

        [HttpGet("confirmation")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Paramètres manquants.");

            var user = await _userRepository.GetByEmail(email);
            if (user == null)
                return NotFound("Utilisateur non trouvé.");

            // Journaliser les valeurs pour le débogage
            Console.WriteLine($"Email reçu : {email}");
            Console.WriteLine($"Token reçu : {token}");
            Console.WriteLine($"Token en DB : {user.ActivationToken}");

            if (user.ActivationToken != token)
                return BadRequest("Jeton invalide.");

            user.IsActive = true;
            user.ActivationToken = null;
            await _userRepository.Update(user);

            return Redirect("/login");
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromForm] string email)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null) return NotFound("Utilisateur non trouvé.");

            // Générer un token de réinitialisation
            user.PasswordResetToken = Guid.NewGuid().ToString();
            await _userRepository.Update(user);

            var resetLink = $"http://localhost:80/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(user.PasswordResetToken)}";
            var subject = "Réinitialisation de votre mot de passe";
            var body = $"<p>Bonjour {user.Username},</p>" +
                    $"<p>Pour réinitialiser votre mot de passe, cliquez sur le lien suivant :</p>" +
                    $"<p><a href='{resetLink}'>Réinitialiser mon mot de passe</a></p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok("Email de réinitialisation envoyé.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] string email, [FromForm] string token, [FromForm] string newPassword)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null) return NotFound("Utilisateur non trouvé.");
            if (user.PasswordResetToken != token) return BadRequest("Jeton invalide.");

            user.PasswordHash = _passWordHasher.HashPassword(newPassword);
            user.PasswordResetToken = null;
            await _userRepository.Update(user);

            return Ok("Mot de passe réinitialisé avec succès.");
        }
    }
}