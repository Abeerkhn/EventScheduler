using EventScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventScheduler.Dtos;
using System.Text.Json.Serialization;
using System.Text.Json;
using EventScheduler.Data;
//using EventScheduler.Migrations;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using EventScheduler.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
//using EventScheduler.Migrations.User;

namespace EventScheduler.Services
{
    public class EventRepository : IEventRepository
    {
        public static Event Event = new Event();
        private readonly IConfiguration _configuration;
        private readonly EventContext _dbContext;
        private readonly UserJoinEventContext _UserJoinEventDbContext;
        private readonly UserContext _UserContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RabbitMQ_Service _rabbitMQService;
        private readonly BlockingCollection<string> messageQueue = new BlockingCollection<string>();

        private readonly IServiceProvider _serviceProvider;


        public EventRepository(EventContext dbContext, UserJoinEventContext userJoinEventContext,
            IHttpClientFactory httpClientFactory,
            UserContext userContext, RabbitMQ_Service rabbitMQ_Service, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _UserJoinEventDbContext = userJoinEventContext;
            _UserContext = userContext;
            _httpClientFactory = httpClientFactory;
            _rabbitMQService = rabbitMQ_Service;
            _serviceProvider = serviceProvider;
        }
        public async Task<Event> CreateEvent(Event eventEntity)
        {
            _dbContext.Events.Add(eventEntity);
            await _dbContext.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<bool> Delete(Event eventName)
        {


            _dbContext.Events.Remove(eventName);
            await _dbContext.SaveChangesAsync();

            return true;
        }




        public async Task<List<Event>> GetAllEvents()
        {

            var events = await _dbContext.Events.ToListAsync();

            foreach (var evnt in events)
            {
                if (evnt.IsRecurring)
                {
                    evnt.RecurrencePattern = await _dbContext.RecurrencePatterns
                        .FirstOrDefaultAsync(rp => rp.Id == evnt.Id);
                }
                else
                {
                    evnt.RecurrencePattern = null;
                }
            }

            return events;
        }


        public async Task<Event> GetEventByTitle(string title)
        {

            return await _dbContext.Events.Include(e => e.RecurrencePattern).FirstOrDefaultAsync(e => e.Title == title);


        }
        public async Task<Event> GetEventByID(Guid id)
        {
            return await _dbContext.Events.Include(e => e.RecurrencePattern).FirstOrDefaultAsync(e => e.Id == id);

        }
        public async Task<bool> UserJoinEvent(Guid Userid, Guid Eventid)
        {
            var USer = GetUserByID(Userid);
            var Event = GetEventByID(Eventid);
            if (USer == null && Event == null)
            {
                return false;
            }

            var eventJoinentity = new UserJoinEvent
            {
                UserId = Userid,
                EventId = Eventid

            };
            _UserJoinEventDbContext.UserEventJoins.Add(eventJoinentity);
            await _UserJoinEventDbContext.SaveChangesAsync();
            return true;

        }

        public bool IsOccurringDaily(Event e)
        {
            if (!e.RecurrencePattern.StartDate.HasValue || !e.RecurrencePattern.Interval.HasValue)
                return false;

            return true;

        }

        public bool IsOccurringMonthly(Event e)
        {
            if (!e.RecurrencePattern.StartDate.HasValue || !e.RecurrencePattern.Interval.HasValue)
                return false;

            return true;

        }

        public bool IsOccurringWeekly(Event e)
        {
            if (!e.RecurrencePattern.StartDate.HasValue || !e.RecurrencePattern.Interval.HasValue)
                return false;

            return true;

        }

        public async Task<bool> SaveChangesAsync()
        {

            try
            {

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions or log the error
                //Return false if saving changes failed
                return false;
            }
        }

        bool IEventRepository.IsRecurringEvent(Event e)
        {
            if (e.RecurrencePattern == null)
                return false;

            if (e.RecurrencePattern.StartDate.HasValue && DateTime.Now < e.RecurrencePattern.StartDate.Value)
                return false;

            if (e.RecurrencePattern.EndDate.HasValue && DateTime.Now > e.RecurrencePattern.EndDate.Value)
                return false;

            switch (e.RecurrencePattern.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    return IsOccurringDaily(e);
                case RecurrenceFrequency.Weekly:
                    return IsOccurringWeekly(e);
                case RecurrenceFrequency.Monthly:
                    return IsOccurringMonthly(e);
                // Add support for more recurrence frequencies as needed
                default:
                    return false;
            }
        }

        public async Task<User> GetUserByID(Guid id)
        {
            var USer = await _UserContext.Users.FirstOrDefaultAsync(e => e.Id == id);
            return USer;
        }

        public async Task<bool> UpdateNotificationPreference(User user)
        {
            await _UserContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUser()
        {
            var Users = await _UserContext.Users.ToListAsync();
            return Users;
        }

        public async Task<List<Event>> GetNearEvents(DateTime Targettime)
        {
            var nonRecurringEvents = await _dbContext.Events
    .Where(e => !e.IsRecurring && e.Time >= Targettime.TimeOfDay && e.Time <= Targettime.AddDays(1).TimeOfDay)
    .ToListAsync();

            // Recurring Events
            var recurringEvents = await _dbContext.Events.Include(e => e.RecurrencePattern)
                .Where(e => e.IsRecurring &&
                            (e.RecurrencePattern.StartDate <= Targettime || e.RecurrencePattern.StartDate == null) &&
                            (e.RecurrencePattern.EndDate >= Targettime || e.RecurrencePattern.EndDate == null))
                .ToListAsync();

            // Filter out the recurring events that occur within the next 24 hours
            recurringEvents = recurringEvents
                .Where(e => IsEventOccurringWithinNext24Hours(e, Targettime))
                .ToList();

            var nearEvents = nonRecurringEvents.Concat(recurringEvents).ToList();
            return nearEvents;


        }

        private bool IsEventOccurringWithinNext24Hours(Event recurringEvent, DateTime targetDateTime)
        {


            // Determine if the recurring event will occur within the next 24 hours
            var startDate = recurringEvent.RecurrencePattern.StartDate.GetValueOrDefault();
            var interval = recurringEvent.RecurrencePattern.Interval.GetValueOrDefault();
            var nextOccurrence = startDate;
            while (nextOccurrence <= targetDateTime)
            {
                nextOccurrence = GetNextOccurrence(nextOccurrence, recurringEvent.RecurrencePattern.Frequency.GetValueOrDefault(), interval);
            }

            return nextOccurrence <= targetDateTime.AddDays(1);
        }

        private DateTime GetNextOccurrence(DateTime startDate, RecurrenceFrequency frequency, int interval)
        {
            switch (frequency)
            {
                case RecurrenceFrequency.Daily:
                    return startDate.AddDays(interval);
                case RecurrenceFrequency.Weekly:
                    return startDate.AddDays(7 * interval);
                case RecurrenceFrequency.Monthly:
                    return startDate.AddMonths(interval);
                //  case RecurrenceFrequency.Yearly:
                //    return startDate.AddYears(interval);
                default:
                    throw new ArgumentException("Unsupported recurrence frequency");
            }
        }


        private async Task<string> GetBearerToken()
        {
            try
            {
                var loginUrl = "http://localhost:7232/api/Auth/login"; // Replace with your actual login endpoint URL

                // Use the IHttpClientFactory to create an instance of HttpClient
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(loginUrl);

                // Create a request body with the user's credentials (username and password)
                var loginCredentials = new
                {
                    Username = "string",
                    Password = "string"
                };

                // Convert the login credentials to JSON
                var content = new StringContent(JsonConvert.SerializeObject(loginCredentials), Encoding.UTF8, "application/json");

                // Send the POST request to the login endpoint
                var response = await httpClient.PostAsync(loginUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content, which should contain the bearer token
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
                    return tokenObject["token"];
                }
                else
                {
                    // Handle authentication failure
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that might occur during the HTTP request
                // Log the error or perform any necessary error handling
                return null;
            }
        }



        private async Task ExecuteGetNearEventsJob()
        {

            try
            {

                var apiUrl = "http://localhost:7232/api/Event/GetNearEvents/near";


                // Use the IHttpClientFactory to create an instance of HttpClient
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var bearerToken = await GetBearerToken();
                Console.WriteLine($"Bearer Token: {bearerToken}");


                // Add authorization header if needed (you may need to modify this based on your authentication method)
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                //  var response = await httpClient.GetAsync(""); // Send a GET req

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var events = JsonConvert.DeserializeObject<List<Event>>(content);
                    var eventIds = events.Select(e => e.Id).ToList();
                    var eventIdsJson = JsonConvert.SerializeObject(eventIds);

                    // Now you can work with the events data
                    // For example, print event titles
                    //var eventsJson = JsonConvert.SerializeObject(events);

                    // Send the events to RabbitMQ
                    var rabbitMQService = _serviceProvider.GetRequiredService<RabbitMQ_Service>();
                    rabbitMQService.SendMessage("event_queue", eventIdsJson);

                    StartMessageConsumption();
                    // The endpoint was successfully executed
                    // You can log the success or perform additional actions if needed
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    // The endpoint returned an error status code
                    Console.WriteLine(errorMessage);
                    // Handle the error or log the failure
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that might occur during the HTTP request
                // Log the error or perform any necessary error handling
            }

        }

        public void ExecuteGetNearEventsJobWrapper()
        {
            ExecuteGetNearEventsJob().Wait();
            // throw new NotImplementedException();
        }

        private async Task Consumemessage()
        {
            _rabbitMQService.ConsumeMessages("event_queue", message =>
            {
                messageQueue.Add(message);
            });
        }
        private void StartMessageProcessingTask()
        {
            //Task.Run(() =>
            //{
                while (!messageQueue.IsCompleted)
                {
                    try
                    {
                      //  messageQueue.Add("test message");
                        var message = messageQueue.Take(); // Blocks until a message is available
                        

                        //using (var scope = _serviceProvider.CreateScope())
                        //{
                        //    var dbContext = scope.ServiceProvider.GetRequiredService<UserJoinEventContext>();
                            ProcessReceivedMessage(message);
                      //  }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions that might occur during message processing
                        // Log the error or perform any necessary error handling
                    }
                }
            //});
        }

        private async void ProcessReceivedMessage(string message)
        {
            var events = JsonConvert.DeserializeObject<List<Guid>>(message);
            try
            {
                // Define a list to store the matching users
                List<UserJoinEvent> matchingUsers = new List<UserJoinEvent>();

                // Iterate over each GUID in the events list
                foreach (Guid ev in events)
                {
                    // Retrieve the users that match the current event ID
                    var usersForEvent = await _UserJoinEventDbContext.UserEventJoins
                        .Where(e => e.EventId == ev)
                        .ToListAsync();

                    // Add the matching users to the list
                    matchingUsers.AddRange(usersForEvent);
                }
                SendNotification(matchingUsers);


                // Process the matching users here, e.g., send notifications based on the user data
              
            }
            catch (Exception ex)
            {
                // Handle exceptions that might occur during processing
                // Log the error or perform any necessary error handling
            }

            Console.WriteLine("Received message: {0}", message);
        }

        public void StartMessageConsumption()
        {

            Task.Run(Consumemessage);
            StartMessageProcessingTask();
        }

        private void SendNotification(List<UserJoinEvent> userJoin) {
            // Step 1: Extract the list of user IDs from the userJoin list
            List<Guid> userIds = userJoin.Select(uje => uje.UserId).Distinct().ToList();

            // Step 2: Use the user IDs to query the database and retrieve the corresponding users
            List<User> users = _UserContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToList();
           
            



        }




    }



}

