
using System.ComponentModel;

namespace Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        [DisplayName("Nom")]
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto()
        {
            Username = string.Empty;
            Email = string.Empty;
        }
    }
}