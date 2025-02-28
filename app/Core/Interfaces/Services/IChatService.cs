namespace Core.Interfaces.Services
{
    public interface IChatService
    {
        // Permet d'envoyer un message
        void SendMessage(string user, string message);

        // Événement déclenché lorsqu'un nouveau message est reçu
        event Action<string> OnMessageReceived;

        // Propriété permettant d'accéder à l'historique des messages
        IReadOnlyList<string> Messages { get; }
    }
}