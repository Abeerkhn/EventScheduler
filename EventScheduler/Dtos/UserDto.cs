using Microsoft.AspNetCore.Identity;

namespace EventScheduler
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string role { get; set; } 
    }
}
