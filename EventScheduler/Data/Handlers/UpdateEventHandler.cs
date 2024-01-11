using AutoMapper;
using Azure.Core;
using EventScheduler.Data.Handlers;
using EventScheduler.Models;
using EventScheduler.Services;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

public class UpdateEventHandler : IRequestHandler<UpdateEventCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    
    public UpdateEventHandler(IEventRepository eventRepository )
    {
        _eventRepository = eventRepository;
        
    }

    public async Task<bool> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var Event = await _eventRepository.GetEventByID(request.Id);
            if (Event == null) { return false; }
            Event.Title = request.Title;
            Event.Description = request.Description;
            Event.CreatedDate = request.CreatedDate;
            Event.Poster = request.Poster;
            Event.Time = request.Time;
            Event.Duration = request.Duration;
            Event.IsRecurring = request.IsRecurring;
            Event.RecurrencePattern = request.IsRecurring ? request.RecurrencePattern : null;
            
            return await _eventRepository.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            return false;
        }

        
    }

}
