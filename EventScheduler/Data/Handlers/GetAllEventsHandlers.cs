using EventScheduler.Data.Query;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class GetAllEventsHandlers : IRequestHandler<GetAllEventsQuery,List<Event>>
    {
        private readonly IEventRepository _eventRepository;
        public GetAllEventsHandlers(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }


        public async Task<List<Event>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            // Implement the logic to retrieve all events from the event repository
            var events = await _eventRepository.GetAllEvents();
            return events;
        }
    }
}
