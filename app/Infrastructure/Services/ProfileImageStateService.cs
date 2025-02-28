using Core.Interfaces.Services;

namespace Infrastructure.Services
{
    /**
    * Service pour stocker l'URL de la photo de profil et notifier les composants.
    */
    public class ProfileImageStateService  : IProfileImageStateService
    {
        // Événement déclenché lors d'un changement d'image
        public event Action OnChange;

        private string _profileImageSrc;
        
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
        
        // Notifie les abonnés du changement d'état
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}