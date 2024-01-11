using EventScheduler.Data.Query;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class GetAllUserHandler : IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly IEventRepository _eventRepository;
        public GetAllUserHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var alluser = await _eventRepository.GetAllUser();
            return alluser;
                    }
    }
}
