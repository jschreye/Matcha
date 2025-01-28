namespace Core.Interfaces.Services
{
    public interface IAlertService
    {
        /// <summary>
        /// Déclenche un message d'alerte.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        /// <param name="isSuccess">Détermine si c'est un succès ou une erreur.</param>
        void ShowMessage(string message, bool isSuccess);

        /// <summary>
        /// Événement déclenché lorsqu'un message est mis à jour.
        /// </summary>
        event Action<string, bool>? OnMessageChanged;
    }
}