using EventScheduler.Data.Query;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class GetNearEventsHandler : IRequestHandler<GetNearEventsQuery, List<Event>>
    {
        private readonly IEventRepository _eventRepository;
        public GetNearEventsHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<List<Event>> Handle(GetNearEventsQuery request, CancellationToken cancellationToken)
        {
            var nearEvents = await _eventRepository.GetNearEvents(request.Targettime);
            return nearEvents;
           
        }
    }
}
