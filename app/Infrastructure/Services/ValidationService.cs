// Infrastructure/Services/ValidationService.cs
using Core.Interfaces.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;
using Core.Interfaces.Repository;
namespace Infrastructure.Services
{
    public class ValidationService : IValidationService
    {
        private const int MaxUsernameLength = 20;
        private const int MaxNameLength = 50;
        private const long MaxImageSizeInBytes = 5 * 1024 * 1024; // 5 MB

        private readonly Regex _usernameRegex = new Regex(@"^[a-zA-Z0-9_]+$");
        private readonly Regex _nameRegex = new Regex(@"^[a-zA-ZÀ-ÿ\s\-]+$"); // Support for accented characters and spaces
        private readonly IUserRepository UserRepository;
        public ValidationService(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }
        public IEnumerable<string> ValidateUsername(string username)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add("Le nom d'utilisateur est requis.");
                return errors;
            }

            if (username.Length > MaxUsernameLength)
            {
                errors.Add($"Le nom d'utilisateur ne doit pas dépasser {MaxUsernameLength} caractères.");
            }

            if (!_usernameRegex.IsMatch(username))
            {
                errors.Add("Le nom d'utilisateur ne doit contenir que des lettres, des chiffres ou des underscores.");
            }

            return errors;
        }

        public IEnumerable<string> ValidateName(string name, string fieldName)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add($"{fieldName} est requis.");
                return errors;
            }

            if (name.Length > MaxNameLength)
            {
                errors.Add($"{fieldName} ne doit pas dépasser {MaxNameLength} caractères.");
            }

            if (!_nameRegex.IsMatch(name))
            {
                errors.Add($"{fieldName} ne doit contenir que des lettres, des espaces ou des tirets.");
            }

            return errors;
        }

        public async Task<IEnumerable<string>> ValidateEmail(string email)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("L'email est requis.");
                return errors;
            }

            var existe = await UserRepository.EmailExistsAsync(email);
            if(existe)
            {
                Console.WriteLine("email exists");
                errors.Add("L'email existe déjà.");
            }

            var emailAttribute = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                errors.Add("L'adresse email est invalide.");
            }

            return errors;
        }

        public IEnumerable<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Le mot de passe est requis.");
                return errors;
            }

            if (password.Length < 8)
            {
                errors.Add("Le mot de passe doit contenir au moins 8 caractères.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errors.Add("Le mot de passe doit contenir au moins une majuscule.");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errors.Add("Le mot de passe doit contenir au moins une minuscule.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                errors.Add("Le mot de passe doit contenir au moins un chiffre.");
            }

            // Ajouter d'autres règles de force du mot de passe si nécessaire

            return errors;
        }
        public IEnumerable<string> ValidateLatitude(double? latitude)
        {
            var errors = new List<string>();

            if (latitude.HasValue)
            {
                if (latitude < -90 || latitude > 90)
                {
                    errors.Add("La latitude doit être comprise entre -90 et 90.");
                }
            }

            return errors;
        }

        public IEnumerable<string> ValidateLongitude(double? longitude)
        {
            var errors = new List<string>();

            if (longitude.HasValue)
            {
                if (longitude < -180 || longitude > 180)
                {
                    errors.Add("La longitude doit être comprise entre -180 et 180.");
                }
            }

            return errors;
        }
        // Validation de la biographie (max 500 caractères)
        public IEnumerable<string> ValidateBiography(string biography)
        {
            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(biography) && biography.Length > 500)
            {
                errors.Add("La biographie ne doit pas dépasser 500 caractères.");
            }

            return errors;
        }

        // Validation d'une image : doit être JPEG ou PNG et ne pas dépasser 5 Mo
        public IEnumerable<string> ValidateImage(IBrowserFile file)
        {
            var errors = new List<string>();

            if (file == null)
                return errors;

            var allowedTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                errors.Add("Le fichier doit être une image au format JPEG ou PNG.");
            }

            if (file.Size > MaxImageSizeInBytes)
            {
                errors.Add("La taille de l'image ne doit pas dépasser 5 Mo.");
            }

            return errors;
        }
    }
}