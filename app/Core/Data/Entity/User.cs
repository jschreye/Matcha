
namespace Core.Data.Entity
{
    public class User
    {
        public int Id { get; set; }
        
        public string Email { get; set; } = string.Empty;
        
        public string Username { get; set; } = string.Empty;
        
        public string Lastname { get; set; } = string.Empty;
        
        public string Firstname { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? Gender { get; set; }
        
        public string? SexualPreferences { get; set; }
        
        public string? Biography { get; set; }
        
        // Représentation de la localisation GPS
        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
        
        public int PopularityScore { get; set; } = 0;
        
        public bool IsActive { get; set; } = false;
        
        public string? ActivationToken { get; set; }
        
        public string? PasswordResetToken { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}