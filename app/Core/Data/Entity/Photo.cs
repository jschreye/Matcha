namespace Core.Data.Entity
{
    public class Photo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public byte[] ImageData { get; set; } = Array.Empty<byte>(); 
        public bool EstProfil { get; set; }

        public virtual User User { get; set; }
    }
}