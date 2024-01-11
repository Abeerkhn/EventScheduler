using MediatR;

namespace EventScheduler.Data.Query
{
    public class GetUserByIDQuery:IRequest<User>
    {
        public Guid UserId { get; set; }
        public GetUserByIDQuery(Guid userid) {
            UserId = userid;
        }
    }
}
