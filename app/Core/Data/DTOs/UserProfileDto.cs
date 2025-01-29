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

    public int? Genre { get; set; }

    public int? SexualPreferences { get; set; }

    public string? Biography { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    // Optionnel : Si vous souhaitez afficher le score de popularité sans le rendre éditable
    public int PopularityScore { get; set; }
    public bool ProfileComplete {get; set;}
}