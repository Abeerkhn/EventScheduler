using EventScheduler.Models;
using MediatR;

namespace EventScheduler.Data.Query
{
    public class GetAllEventsQuery:IRequest<List<Event>>
    {
    }
}
