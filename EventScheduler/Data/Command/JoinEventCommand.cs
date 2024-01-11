using MediatR;

namespace EventScheduler.Data.Command
{
    public class JoinEventCommand:IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }
}
