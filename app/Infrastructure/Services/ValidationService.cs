// Infrastructure/Services/ValidationService.cs
using Core.Interfaces.Services;
// using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    public class ValidationService : IValidationService
    {
        private const int MaxUsernameLength = 20;
        private const int MaxNameLength = 50;
        private const long MaxImageSizeInBytes = 2 * 1024 * 1024; // 2 MB

        private readonly Regex _usernameRegex = new Regex(@"^[a-zA-Z0-9_]+$");
        private readonly Regex _nameRegex = new Regex(@"^[a-zA-ZÀ-ÿ\s\-]+$"); // Support for accented characters and spaces

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

        public IEnumerable<string> ValidateEmail(string email)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("L'email est requis.");
                return errors;
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

    //     public IEnumerable<string> ValidateImage(IFileListEntry image, long maxSizeInBytes)
    //     {
    //         var errors = new List<string>();

    //         if (image == null)
    //         {
    //             errors.Add("Aucune image téléchargée.");
    //             return errors;
    //         }

    //         if (image.Size > maxSizeInBytes)
    //         {
    //             errors.Add($"L'image ne doit pas dépasser {maxSizeInBytes / (1024 * 1024)} MB.");
    //         }

    //         // Vérifier le type de fichier si nécessaire
    //         var allowedTypes = new List<string> { "image/jpeg", "image/png", "image/gif" };
    //         if (!allowedTypes.Contains(image.Type))
    //         {
    //             errors.Add("Le format de l'image n'est pas pris en charge. Formats autorisés : JPEG, PNG, GIF.");
    //         }

    //         return errors;
    //     }
    }
}