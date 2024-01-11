using EventScheduler.Data.Command;
using EventScheduler.Data.Handlers;
using EventScheduler.Data.Query;
using EventScheduler.Dtos;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.Design;

namespace EventScheduler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    [Authorize]
    public class EventController : Controller
    {
        public static Event Event = new Event();
        private readonly IConfiguration _configuration;
        private readonly EventContext _dbContext;
        private readonly IMediator _mediator;





        public EventController(EventContext dbContext, IConfiguration configurtion, IMediator mediator)
        {
            _dbContext = dbContext;
            _configuration = configurtion;
            _mediator = mediator;

        }


        [HttpPost("CQRS_CREATE_EVENT")]
        public async Task<ActionResult<EventResponsedto>> CQRS_Create(Eventdto request)
        {
            RecurrencePattern recurrencePattern = null;

            if (request.IsRecurring)
            {
                recurrencePattern = new RecurrencePattern
                {
                    Frequency = request.RecurrencePattern.Frequency,
                    Interval = request.RecurrencePattern.Interval,
                    StartDate = request.RecurrencePattern.StartDate,
                    EndDate = request.RecurrencePattern.EndDate
                };
            }
            else
            {
                recurrencePattern = null; // Set recurrencePattern to null for non-recurring events
            }
            var createEventCommand = new CreateEventCommand(
           request.Title,
           request.Description,
           request.CreatedDate,
           request.Time,
           request.Poster,
           request.Duration,
           request.IsRecurring,
           recurrencePattern
           );

            var createdEvent = await _mediator.Send(createEventCommand);

           

            var eventResponseDto = new EventResponsedto
            {
                Title = createdEvent.Title
                // Include other properties as needed
            };

            return Ok(eventResponseDto);


        }


        [HttpPost("CreateEvent")]
        // Post=  CreateEvent

        public async Task<ActionResult<EventResponsedto>> Create(Eventdto request)
        {
            var eventSearch = await _dbContext.Events.FirstOrDefaultAsync(e => e.Title == request.Title);
            if (eventSearch != null)
            {
                return BadRequest("Event Already exists");
            }


            var eventEntity = new Event
            {
                Title = request.Title,
                Description = request.Description,
                CreatedDate = request.CreatedDate,
                Time = request.Time,
                Poster = request.Poster,
                Duration = request.Duration,
                IsRecurring = request.IsRecurring
            };

            if (request.IsRecurring)
            {
                var recurrencePattern = new RecurrencePattern
                {
                    Frequency = request.RecurrencePattern.Frequency,
                    Interval = request.RecurrencePattern.Interval,
                    StartDate = request.RecurrencePattern.StartDate,
                    EndDate = request.RecurrencePattern.EndDate
                };

                eventEntity.RecurrencePattern = recurrencePattern;
            }

            _dbContext.Events.Add(eventEntity);
            await _dbContext.SaveChangesAsync();

            var eventResponseDto = new EventResponsedto
            {
                Title = eventEntity.Title
            };

            return Ok(eventResponseDto);
        }


      

        
      
        [HttpGet("/GET_ALL_EVENTS")]
        public async Task<IActionResult> Get_All_Events()
        {
            var query = new GetAllEventsQuery();
            var events = await _mediator.Send(query);
           
            return Ok(events);
        }

        [HttpGet("GET_EVENTS_BY_TITLE")]
        public async Task<IActionResult> Get_Events_By_Title( string EventTitle)
        {
            var query = new GetEventByTitleQuery(EventTitle);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result); // Event successfully deleted

        }

        [HttpGet("GET_EVENTS_BY_ID")]
        public async Task<IActionResult> Get_Events_By_ID(Guid ID)
        {
            var query = new GetEventByIDQuery(ID);
            var result = await _mediator.Send(query);
            if(result == null) { return NotFound(); }
            return Ok(result);

        }


        [HttpPost("Join")]
        public async Task<ActionResult> JoinEvent(Guid Userid,Guid Eventid)
        {
            var joinentity = new JoinEventCommand
            {
               UserId = Userid,

                EventId=Eventid
            };
            var result = await _mediator.Send(joinentity);
            if (result == false)
            {
                return NotFound();
            }
            return Ok();
        }


        [HttpPut("{userId}/notification-preferences")]
        public async Task<IActionResult> UpdateNotificationPreferences(Guid userId, [FromBody] NotificationPreferenceDTO preferencesDto)
        {
            try
            {
                var command = new UpdateNotificationPreferenceCommand
                {
                    UserID = userId,
                    Notifications = preferencesDto.NotificationsPreferences
                };
                var result = await _mediator.Send(command);

                if (result)
                {
                    return Ok("Notification preferences updated successfully.");
                }
                else
                {
                    return NotFound(); // Or return appropriate error response if the user is not found
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                return StatusCode(500, "An error occurred while updating notification preferences.");
            }
        }




        
       

        // GET: EventController/Delete/5

        [HttpDelete("delete/{eventName}")]



        public async Task<IActionResult> Delete(string eventName)
        {
            var eventEntity = await _dbContext.Events.FirstOrDefaultAsync(e => e.Title == eventName);
            if (eventEntity == null)
            {
                return NotFound(); // Event not found
            }

            _dbContext.Events.Remove(eventEntity);
            await _dbContext.SaveChangesAsync();

            return Ok(); // Event successfully deleted
        }




        [HttpDelete("CQRS_delete/{eventName}")]

        public async Task<ActionResult> CQRS_Delete(string eventName)
        {
            var DeleteEventCommand = new DeleteEventCommand(eventName);

            var deletionevent = await _mediator.Send(DeleteEventCommand);
            if (!deletionevent)
            {
                return NotFound(); // Event not found or deletion failed
            }
            return Ok();


        }

        [HttpDelete("/ID_DELETE")]
        public async Task<ActionResult> Delete_W_ID(Guid Id)
        {
            var DeleteEventIDCommand =  new DeleteEventIDCommand(Id);
            var deletionevent = await _mediator.Send(DeleteEventIDCommand);
            if (!deletionevent)
            {
                return NotFound(); // Event not found or deletion failed
            }
            return Ok();

        }


        [HttpGet("Get_All_Users")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var Query = new GetAllUsersQuery();
                var result =await _mediator.Send(Query);
              /*  if (result == null)
                {
                    return NotFound();

                }*/
                return Ok(result);
                 }
            catch (Exception ex) {
                return NotFound(ex.Message);
                    };
            }



        [HttpGet("Get_USER_BY_ID")]
        public async Task<ActionResult> GetUserByID(Guid UserID)
        {
            var query = new GetUserByIDQuery(UserID);
           
            var result = await _mediator.Send(query); 
            return Ok(result);
        }

        [HttpPut("/ID_Update")]
        public async Task<ActionResult> Update_event(Guid Id, [FromBody] Event update_event)
        {
           
            var UpdateEventCommand = new UpdateEventCommand(Id,
                update_event.Title,
                update_event.Description,    
                update_event.CreatedDate,
                update_event.Duration,
                update_event.Poster,
                update_event.Time,
                update_event.IsRecurring,
                update_event.RecurrencePattern
                );
            var send_mediator=await _mediator.Send(UpdateEventCommand);
            if (!send_mediator)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("near")]
        public async Task<ActionResult<List<Event>>> GetNearEvents()
        {
            var targetDateTime = DateTime.Now; // or specify any other target date/time
            var query =  new GetNearEventsQuery { Targettime = targetDateTime};
            var nearEvents = await _mediator.Send(query);
            if (nearEvents.Count==0)
            {
                    return NotFound("Empty List");
            }
            else
            {
                return Ok(nearEvents);
            }
        }

    }
}
