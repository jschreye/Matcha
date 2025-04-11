namespace Core.Interfaces.Services
{
    public interface IProfileImageStateService
    {
        // Événement déclenché lors d'un changement d'image.
        event Action OnChange;
        
        // Événement déclenché lors d'un changement d'image pour un utilisateur spécifique
        event Action<int> OnUserImageChange;

        // Propriété de l'image de profil (ancienne méthode, gardée pour compatibilité).
        string ProfileImageSrc { get; set; }
        
        // Nouvelles méthodes pour gérer les images par utilisateur
        string GetUserProfileImage(int userId);
        void SetUserProfileImage(int userId, string imageSrc);
    }
}