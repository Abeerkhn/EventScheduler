using EventScheduler.Models;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
namespace EventScheduler.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<JsonPatchDocument<Event>, Event>();
            CreateMap<JsonPatchDocument<RecurrencePattern>, RecurrencePattern>();
        }
    }
}
