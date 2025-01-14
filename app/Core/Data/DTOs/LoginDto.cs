using System.ComponentModel.DataAnnotations;

namespace Core.Data.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Le mot de passe est requis.")]
        public string Password { get; set; }
    }
}