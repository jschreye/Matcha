namespace Core.Interfaces.Services
{
    public interface IProfileImageStateService
    {
        // Événement déclenché lors d'un changement d'image.
        event Action OnChange;

        // Propriété de l'image de profil.
        string ProfileImageSrc { get; set; }
    }
}