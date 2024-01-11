using EventScheduler.Data.Command;
using EventScheduler.Services;
using MediatR;

public class JoinEventHandler : IRequestHandler<JoinEventCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    public JoinEventHandler(
        IEventRepository eventRepository
        )
    {
        _eventRepository = eventRepository;
    }

    public async Task<bool> Handle(JoinEventCommand request, CancellationToken cancellationToken)
    {
        return await _eventRepository.UserJoinEvent(request.UserId, request.EventId);
        
    }
}
