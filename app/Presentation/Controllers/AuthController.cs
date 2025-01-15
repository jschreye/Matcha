using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Presentation.Controllers // Remplacez par votre espace de noms approprié
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            if (await _userService.ValidateUser(username, password))
            {
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
    }
}