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

        public AuthController(IUserService userService, IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
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
    }
}