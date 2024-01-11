using EventScheduler.Dtos;
using Microsoft.AspNetCore.Mvc;
using EventScheduler.Models;
using Microsoft.AspNetCore.Components.Web;

namespace EventScheduler.Services
{
    public interface IEventRepository
    {
        // Commands
        public Task<Event> CreateEvent(Event request);
        public Task<bool> Delete(Event eventName);
      
        public Task<Event> GetEventByTitle(string title);
        public Task<Event> GetEventByID(Guid Id);
        public bool IsRecurringEvent(Event e);
        public bool IsOccurringDaily(Event e);
        public bool IsOccurringWeekly(Event e);
        public bool IsOccurringMonthly(Event e);
        public Task<List<Event>> GetAllEvents();
        public Task<List<User>> GetAllUser();
        public Task<User> GetUserByID(Guid id);
        public Task<bool> UpdateNotificationPreference(User user);
        public Task<bool> UserJoinEvent(Guid USerId,Guid EventId);
        public Task<List<Event>> GetNearEvents(DateTime Targettime);
        public Task<bool> SaveChangesAsync();
        public void ExecuteGetNearEventsJobWrapper();


    }
}
