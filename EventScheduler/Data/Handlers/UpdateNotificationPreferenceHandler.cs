using EventScheduler.Data.Command;
using EventScheduler.Services;
using MediatR;

namespace EventScheduler.Data.Handlers
{
    public class UpdateNotificationPreferenceHandler : IRequestHandler<UpdateNotificationPreferenceCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        public UpdateNotificationPreferenceHandler(IEventRepository eventRepository) {
        _eventRepository = eventRepository;
           
        }

        public async Task<bool> Handle(UpdateNotificationPreferenceCommand request, CancellationToken cancellationToken)
        {
            var user = await _eventRepository.GetUserByID(request.UserID);
            if (user == null) { return false; }
            user.NotificationsPreferences = request.Notifications;
          return  await _eventRepository.UpdateNotificationPreference(user);

      
        }
    }
}
