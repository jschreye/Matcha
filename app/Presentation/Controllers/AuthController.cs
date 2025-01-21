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
                if(user == null || !user.IsActive) 
                    return Unauthorized("Identifiants incorrects ou compte non actif.");

                // Créer les claims et l'identité pour le cookie d'authentification
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                    // Ajoutez d'autres claims si nécessaire
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                Console.WriteLine("Utilisateur validé. Tentative de création du cookie d'authentification.");

                // Créer le cookie d'authentification
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                Console.WriteLine("Cookie d'authentification créé.");

                // Gérer la session
                var sessionToken = Guid.NewGuid().ToString();
                var expiresAt = DateTime.UtcNow.AddHours(4);

                // Stocker la session en base
                await _userService.CreateSessionAsync(user.Id, sessionToken, expiresAt);

                // Créer le cookie de session
                Response.Cookies.Append("SessionToken", sessionToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expiresAt
                });

                return Ok(new { Message = "Connexion réussie" });
            }

            Console.WriteLine("Identifiants incorrects");
            return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("SessionToken", out var sessionToken))
            {
                await _userService.DeleteSessionAsync(sessionToken);
                Response.Cookies.Delete("SessionToken");
            }
            
            // Déconnexion de l'authentification
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

            // Utilisation de l'opérateur de null-forgiveness car nous sommes sûrs que ces valeurs ne sont pas nulles
            var escapedEmail = Uri.EscapeDataString(user.Email!);
            var escapedToken = Uri.EscapeDataString(user.PasswordResetToken!);

            var resetLink = $"http://localhost:80/reset-password?email={escapedEmail}&token={escapedToken}";
            var subject = "Réinitialisation de votre mot de passe";
            var body = $"<p>Bonjour {user.Username},</p>" +
                       $"<p>Pour réinitialiser votre mot de passe, cliquez sur le lien suivant :</p>" +
                       $"<p><a href='{resetLink}'>Réinitialiser mon mot de passe</a></p>";

            await _emailService.SendEmailAsync(user.Email!, subject, body);

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