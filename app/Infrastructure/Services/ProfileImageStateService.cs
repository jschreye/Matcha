using Core.Interfaces.Services;

namespace Infrastructure.Services
{
    /**
    * Service pour stocker les URLs des photos de profil par utilisateur et notifier les composants.
    */
    public class ProfileImageStateService : IProfileImageStateService
    {
        // Événement déclenché lors d'un changement d'image
        public event Action OnChange = delegate { };
        public event Action<int> OnUserImageChange = delegate { };

        // Dictionnaire qui stocke les images de profil par utilisateur
        private readonly Dictionary<int, string> _userProfileImages = new();
        
        // Pour la compatibilité avec le code existant
        private string _profileImageSrc = string.Empty;
        
        // Propriété de l'image de profil qui notifie lors d'un changement
        public string ProfileImageSrc
        {
            get => _profileImageSrc;
            set
            {
                if (_profileImageSrc != value)
                {
                    _profileImageSrc = value;
                    NotifyStateChanged();
                }
            }
        }
        
        // Méthodes pour gérer les images par utilisateur
        public string GetUserProfileImage(int userId)
        {
            return _userProfileImages.TryGetValue(userId, out var imageSrc) ? imageSrc : string.Empty;
        }
        
        public void SetUserProfileImage(int userId, string imageSrc)
        {
            if (!_userProfileImages.TryGetValue(userId, out var currentImage) || currentImage != imageSrc)
            {
                _userProfileImages[userId] = imageSrc;
                NotifyUserImageChanged(userId);
                
                // Pour la compatibilité avec le code existant
                ProfileImageSrc = imageSrc;
            }
        }
        
        // Notifie les abonnés du changement d'état
        private void NotifyStateChanged() => OnChange?.Invoke();
        
        // Notifie les abonnés du changement d'image pour un utilisateur spécifique
        private void NotifyUserImageChanged(int userId) => OnUserImageChange?.Invoke(userId);
    }
}