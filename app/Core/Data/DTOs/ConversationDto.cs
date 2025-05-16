namespace Core.Data.DTOs
{
    public class ConversationDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Photo { get; set; }
        public string? LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
    }
}