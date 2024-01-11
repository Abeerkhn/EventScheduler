using EventScheduler.Models;
using MediatR;

namespace EventScheduler.Data.Query
{
    public class GetNearEventsQuery:IRequest<List<Event>>
    {
        public DateTime Targettime { get; set; }
    }
}

