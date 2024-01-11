using EventScheduler.Controllers;
using EventScheduler.Models;

namespace EventScheduler.Dtos
{
    public class Eventdto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan Time { get; set; }
        public string Poster { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsRecurring { get; set; }
        public RecurrencePatterndto? RecurrencePattern { get; set; }
    }
   


}
