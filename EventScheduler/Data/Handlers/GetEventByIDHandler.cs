using EventScheduler.Data.Query;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class GetEventByIDHandler : IRequestHandler<GetEventByIDQuery, Event>
    {

        private readonly IEventRepository _eventRepository;

        public GetEventByIDHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<Event> Handle(GetEventByIDQuery request, CancellationToken cancellationToken)
        {
            return await _eventRepository.GetEventByID(request.Id);
        }
    }
}
