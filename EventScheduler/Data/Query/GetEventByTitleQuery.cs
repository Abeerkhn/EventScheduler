using MediatR;
using EventScheduler.Models;

namespace EventScheduler.Data.Query
{
    public class GetEventByTitleQuery:IRequest<Event>
    {
        public string Title { get; set; }
        public GetEventByTitleQuery(string title)
        {
            Title = title;
        }
    }
}
