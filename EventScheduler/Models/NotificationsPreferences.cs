namespace EventScheduler.Models
{
    [Flags]
    public enum NotificationsPreferences
    {
        None = 0,
        Email = 1,
        Push = 2,
        SMS = 4
    }
}
