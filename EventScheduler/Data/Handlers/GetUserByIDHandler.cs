using EventScheduler.Data.Query;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class GetUserByIDHandler:IRequestHandler<GetUserByIDQuery,User>
    {
        private readonly IEventRepository _eventRepository;
        public GetUserByIDHandler(IEventRepository eventRepository) { 
        _eventRepository = eventRepository; 
        }

        public async Task<User> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
        {
            return await _eventRepository.GetUserByID(request.UserId);
            
        }
    }
}
