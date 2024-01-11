using EventScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventScheduler
{
    public class User
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]

        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid phone number format. Use digits only.")]
        public string Phone { get; set; }
        public string role { get; set; } = "User";

        [JsonIgnore]
        public byte[] Passwordhash { get; set; }

        [JsonIgnore]
        public byte[] Passwordsalt { get; set; }


        [JsonIgnore]
        [NotMapped]
        public ICollection<UserJoinEvent>? UserJoinEvents { get; set; } = new List<UserJoinEvent>();
        
        public NotificationsPreferences? NotificationsPreferences { get; set; }

    }
}




