using Core.Interfaces.Services;

namespace Infrastructure.Services
{
    public class AlertService : IAlertService
    {
        public event Action<string, bool>? OnMessageChanged;

        /// <summary>
        /// Déclenche un message d'alerte.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        /// <param name="isSuccess">Détermine si c'est un succès ou une erreur.</param>
        public void ShowMessage(string message, bool isSuccess)
        {
            OnMessageChanged?.Invoke(message, isSuccess);
        }
    }
}