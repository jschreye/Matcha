using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Entity;
using Core.Interfaces.Services;
public class ChatService : IChatService
{
    private readonly List<Message> _messages = new List<Message>();
    public IReadOnlyList<Message> Messages => _messages;

    public event Action<Message>? OnMessageReceived;

    public void SendMessage(int senderId, int receiverId, string contenu)
    {
        var msg = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Contenu = contenu,
            Timestamp = DateTime.Now
        };

        _messages.Add(msg);
        OnMessageReceived?.Invoke(msg);
    }
}
