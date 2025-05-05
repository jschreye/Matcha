using System.ComponentModel.DataAnnotations;

public class UserProfileDto
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Lastname { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Firstname { get; set; } = string.Empty;

    public int Age { get; set; }
    public int? Genre { get; set; }
    public int? Tag { get; set; }
    public int? SexualPreferences { get; set; }

    public string? Biography { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
    public int PopularityScore { get; set; }    
    public bool ProfileComplete {get; set;}
    public bool NotifIsActive {get; set;}
    public bool IsActive {get; set;}
    public bool LocalisationIsActive { get; set; }
    public DateTime LastActivity {get; set;}
}