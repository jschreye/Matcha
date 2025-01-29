namespace Core.Data.Entity
{
    public class BlockReport
    {
        // Clé primaire composite : (user_id, blocked_user_id)
        public int UserId { get; set; }
        public int BlockedUserId { get; set; }

        // Raison du report, texte libre
        public string? ReportReason { get; set; }

        // Date/heure de l'action
        public DateTime Timestamp { get; set; }
    }
}