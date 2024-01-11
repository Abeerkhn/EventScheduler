using MediatR;

namespace EventScheduler.Data.Command
{
    public class DeleteEventIDCommand:IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteEventIDCommand(Guid id)
        {
            Id = id;
        }
    }
}
