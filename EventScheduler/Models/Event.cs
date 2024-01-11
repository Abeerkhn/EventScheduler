using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventScheduler.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }= Guid.NewGuid();
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
       
        [MaxLength(200)]
        public string Description { get; set; }
      
        public DateTime CreatedDate { get; set; }=DateTime.Now;
        public TimeSpan Time { get; set; }=TimeSpan.Zero;
        public string Poster { get; set; }
        public TimeSpan Duration { get; set;}
     
        public bool IsRecurring { get; set; }
        // Additional properties for recurring events
        public RecurrencePattern? RecurrencePattern { get; set; }
        //[JsonIgnore]
        //  public ICollection<User> Users { get; set; } = new List<User>();
        [JsonIgnore]
        [NotMapped]
        public ICollection<UserJoinEvent>? UserJoinEvents { get; set; } = new List<UserJoinEvent>(); 




    }
}
