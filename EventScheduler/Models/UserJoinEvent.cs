using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventScheduler.Models
{
    public class UserJoinEvent
    {
        [Key]
        public Guid UJEId { get; set; }
        public Guid UserId { get; set; }
        [NotMapped]
        public User User { get; set; }

        public Guid EventId { get; set; }
        [NotMapped]
        public Event Event { get; set; }
    }
}
