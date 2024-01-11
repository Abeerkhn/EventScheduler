using EventScheduler.Models;

namespace EventScheduler.Dtos
{
    public class RecurrencePatterndto
    {
        public RecurrenceFrequency? Frequency { get; set; }
        public int? Interval { get; set; } = 1;
        public DateTime? StartDate { get; set; }
        public DateTime?  EndDate { get; set; }
    }
}
