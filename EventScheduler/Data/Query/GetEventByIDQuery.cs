using EventScheduler.Models;
using MediatR;

namespace EventScheduler.Data.Query
{
    public class GetEventByIDQuery:IRequest<Event>
    {
        public Guid Id { get; set; }
        public GetEventByIDQuery(Guid id)
        {
            Id = id;
        }
    }
}
