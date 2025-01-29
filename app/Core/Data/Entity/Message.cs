namespace Core.Data.Entity
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Contenu { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}