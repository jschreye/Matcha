// Core/Interfaces/Services/IValidationService.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IValidationService
    {
        IEnumerable<string> ValidateUsername(string username);
        IEnumerable<string> ValidateName(string name, string fieldName);
        IEnumerable<string> ValidateEmail(string email);
        IEnumerable<string> ValidatePassword(string password);
        IEnumerable<string> ValidateLatitude(double? latitude);
        IEnumerable<string> ValidateLongitude(double? longitude);
        IEnumerable<string> ValidateBiography(string biography);
        IEnumerable<string> ValidateImage(IBrowserFile file);

    }

}