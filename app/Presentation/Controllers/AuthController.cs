using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;

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

                await _userRepository.UpdateLastActivityAsync(user.Id);

                // Créer les claims et l'identité pour le cookie d'authentification
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("ProfileComplete", user.ProfileComplete ? "true" : "false")
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
                #if DEBUG
                    var cookieSecure = false;
                #else
                    var cookieSecure = true;
                #endif
                // Créer le cookie de session
                Response.Cookies.Append("SessionToken", sessionToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure   = cookieSecure,
                    SameSite = SameSiteMode.Strict,
                    Expires  = expiresAt,
                    Path     = "/"
                });

                return Ok(new { Message = "Connexion réussie" });
            }

            Console.WriteLine("Identifiants incorrects");
            return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine($"🔐 logout");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim?.Value, out var userId))
            {
                Console.WriteLine($"🔐 Logout pour userId = {userId}");
                // 1) Supprimer TOUTES les sessions en base
                await _userService.DeleteSessionsByUserIdAsync(userId);

                // 2) Effacer le cookie côté client
                Response.Cookies.Delete("SessionToken", new CookieOptions {
                    HttpOnly = true,
                    Secure   = false,
                    SameSite = SameSiteMode.Strict,
                    Path     = "/"
                });

                // 3) Mettre à jour last_activity
                await _userRepository.UpdateLastActivityAsync(userId);
            }

            // 4) Sign‐out du cookie d’authentification ASP.NET
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 5) Redirection
            return Redirect("/login");
        }

        [HttpGet("confirmation")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Paramètres manquants.");

            var user = await _userRepository.GetByEmail(email);
            if (user == null)
                return NotFound("Utilisateur non trouvé.");

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
            if (user == null) return NotFound(new { Message = "Utilisateur non trouvé." });

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

            return Ok(new { Message = "Email de réinitialisation envoyé." });
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
        [HttpPost("refresh-profile-claim")]
        public async Task<IActionResult> RefreshProfileClaim()
        {
            // Récupérer l'user ID depuis le claim NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Pas d'utilisateur.");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest("ID user invalide");

            // Relire l'utilisateur en base (pour connaître l'état ProfileComplete)
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound("Utilisateur introuvable.");

            // Reconstruire les claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("ProfileComplete", user.ProfileComplete ? "true" : "false")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Regénérer le cookie d'authentification
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // On peut renvoyer l'info au client
            return Ok(new { ProfileComplete = user.ProfileComplete });
        }

        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            // Récupérer l'user ID depuis le claim NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Pas d'utilisateur authentifié.");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest("ID utilisateur invalide");

            // Récupérer l'utilisateur depuis la base de données
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound("Utilisateur introuvable.");

            // Renvoyer les informations nécessaires
            return Ok(new { 
                id = user.Id,
                username = user.Username,
                profileComplete = user.ProfileComplete,
                localisationIsActive = user.LocalisationIsActive
            });
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateModel model)
        {
            // Récupérer l'user ID depuis le claim NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Pas d'utilisateur authentifié.");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest("ID utilisateur invalide");

            // Mettre à jour la localisation de l'utilisateur avec le statut de géolocalisation
            await _userService.UpdateLocationAsync(userId, model.Latitude, model.Longitude, model.LocalisationIsActive);
            
            return Ok(new { success = true });
        }
    }

    public class LocationUpdateModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool LocalisationIsActive { get; set; }
    }
}