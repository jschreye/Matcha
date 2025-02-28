using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Entity;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;
public class ChatService : IChatService
{
    private readonly List<Message> _messages = new List<Message>();
    private readonly IMessageRepository _messageRepository;

    public ChatService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public IReadOnlyList<Message> Messages => _messages;
    public event Action<Message>? OnMessageReceived;

    public async Task SendMessageAsync(int senderId, int receiverId, string contenu)
    {
        var msg = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Contenu = contenu,
            Timestamp = DateTime.Now
        };

        // Sauvegarder le message en base
        await _messageRepository.SaveMessageAsync(msg);

        // Ajout en mémoire et notification
        _messages.Add(msg);
        OnMessageReceived?.Invoke(msg);
    }

    public async Task<List<Message>> LoadConversationAsync(int userId1, int userId2)
    {
        return await _messageRepository.GetConversationAsync(userId1, userId2);
    }
}
