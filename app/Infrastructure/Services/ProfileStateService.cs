using System;

namespace Infrastructure.Services
{
    /** 
     * Service pour gérer l'état de l'image de profil et notifier les composants abonnés.
     */
    public class ProfileStateService
    {
        /** 
         * Événement déclenché lors d'un changement d'image de profil.
         */
        public event Action? OnChange;

        private string? _profileImageSrc;

        /** 
         * Propriété contenant l'URL de l'image de profil.
         */
        public string? ProfileImageSrc
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

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}