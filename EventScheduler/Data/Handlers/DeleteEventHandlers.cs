using EventScheduler.Data.Command;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EventScheduler.Data.Handlers
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;

        public DeleteEventCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            // Find the event by its title
            var eventToDelete = await _eventRepository.GetEventByTitle(request.EventTitle);
            if (eventToDelete == null)
            {
                // Event not found, return false indicating deletion failure
                return false;
            }

            // Delete the event
            var deletionResult = await _eventRepository.Delete(eventToDelete);

            // Return true if the deletion was successful, otherwise false
            return deletionResult;
        }
    }
}
