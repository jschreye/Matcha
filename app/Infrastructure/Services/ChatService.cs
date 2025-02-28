using Core.Interfaces.Services;
using System;
using System.Collections.Generic;

public class ChatService : IChatService
{
    // Stockage interne des messages
    private readonly List<string> _messages = new List<string>();

    // Implémentation de la propriété avec IReadOnlyList<string>
    public IReadOnlyList<string> Messages => _messages;

    public event Action<string>? OnMessageReceived;

    public void SendMessage(string user, string message)
    {
        var fullMessage = $"{user}: {message}";
        _messages.Add(fullMessage);
        OnMessageReceived?.Invoke(fullMessage);
    }
}