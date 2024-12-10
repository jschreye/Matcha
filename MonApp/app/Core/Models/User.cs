
namespace Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public User()
        {
            Username = string.Empty;
            Email = string.Empty;
        }
    }
}