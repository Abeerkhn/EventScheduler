using EventScheduler.Data.Command;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Event>
    {
        private readonly IEventRepository _eventRepository;

        public CreateEventCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Event> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
          
            RecurrencePattern recurrencePattern = null;
            var eventEntity = new Event
            {
                Title = request.Title,
                Description = request.Description,
                CreatedDate = request.CreatedDate,
                Time = request.Time,
                Poster = request.Poster,
                Duration = request.Duration,
                IsRecurring = request.IsRecurring
            };
            bool isRecurring = _eventRepository.IsRecurringEvent(eventEntity);

            if (request.IsRecurring)
            {
                 recurrencePattern = new RecurrencePattern
                {
                    Frequency = request.RecurrencePattern.Frequency,
                    Interval = request.RecurrencePattern.Interval,
                    StartDate = request.RecurrencePattern.StartDate,
                    EndDate = request.RecurrencePattern.EndDate
                };

                eventEntity.RecurrencePattern = recurrencePattern;
            }

            return await _eventRepository.CreateEvent(eventEntity);
        }
    }

}
