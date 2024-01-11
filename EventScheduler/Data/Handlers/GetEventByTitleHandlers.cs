using MediatR;
using EventScheduler.Models;
using System.Threading;
using System.Threading.Tasks;
using EventScheduler.Services;
using EventScheduler.Data.Query;

namespace EventScheduler.Data.Handlers
{
    public class GetEventByTitleHandler : IRequestHandler<GetEventByTitleQuery, Event>
    {
        private readonly IEventRepository _eventRepository;

        public GetEventByTitleHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Event> Handle(GetEventByTitleQuery request, CancellationToken cancellationToken)
        {
           
            return await _eventRepository.GetEventByTitle(request.Title);
        }
    }
}
