using EventScheduler.Dtos;
using EventScheduler.Models;
using MediatR;

namespace EventScheduler.Data.Command
{
    public class CreateEventCommand : IRequest<Event>
    {
        public CreateEventCommand(string title,
        string description,
        DateTime createdDate,
        TimeSpan time,
        string poster,
        TimeSpan duration,
        bool isRecurring,   
        RecurrencePattern recurrencePattern) {
            Title = title;
            Description = description;
            CreatedDate = createdDate;
            Time = time;
            Poster = poster;
            Duration = duration;
            IsRecurring = isRecurring;
            RecurrencePattern = recurrencePattern;
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan Time { get; set; }
        public string Poster { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsRecurring { get; set; }
        public RecurrencePattern? RecurrencePattern { get; set; }
        
    }
}
