using EventScheduler.Data.Command;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class DeleteEventIDHandler:IRequestHandler<DeleteEventIDCommand,bool>
    {
        private readonly IEventRepository _eventRepository;

        public DeleteEventIDHandler(IEventRepository eventRepository) {
        _eventRepository = eventRepository;
        }
        public async Task<bool> Handle(DeleteEventIDCommand request, CancellationToken cancellationToken)
        {
            var eventToDelete = await _eventRepository.GetEventByID(request.Id);
            if (eventToDelete == null)
            {
                // Event not found, return false indicating deletion failure
                return false;
            }
            var deletionResult = await _eventRepository.Delete(eventToDelete);
            return true;
        }
    }
}
