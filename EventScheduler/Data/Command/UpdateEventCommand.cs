using MediatR;
using EventScheduler.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace EventScheduler.Data.Handlers
{
    public class UpdateEventCommand : IRequest<bool> 
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan Time { get; set; }
        public string Poster { get; set; }
        public TimeSpan Duration { get; set; }

        public bool IsRecurring { get; set; }

        public RecurrencePattern? RecurrencePattern { get; set; }

        public UpdateEventCommand(Guid id, string title, string description, DateTime createdDate, TimeSpan time, string poster, TimeSpan duration, bool isRecurring, RecurrencePattern? recurrencePattern)
        {
            Id = id;
            Title = title;
            Description = description;
            CreatedDate = createdDate;
            Time = time;
            Poster = poster;
            Duration = duration;
            IsRecurring = isRecurring;  
            RecurrencePattern = recurrencePattern;
        }
    }
}
