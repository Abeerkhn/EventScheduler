using EventScheduler.Models;
using MediatR;

namespace EventScheduler.Data.Command
{
    public class UpdateNotificationPreferenceCommand:IRequest<bool>
    {
        public Guid UserID { get; set; }
        public NotificationsPreferences Notifications { get; set; }
    }
}
