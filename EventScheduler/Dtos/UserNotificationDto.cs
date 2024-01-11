using EventScheduler.Models;

namespace EventScheduler.Dtos
{
    public class UserNotificationDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public NotificationsPreferences NotificationPreference { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
