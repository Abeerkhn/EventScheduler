using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventScheduler.Models
{
    public class RecurrencePattern
    {
        public Guid Id { get; set; }//Fk
        public Guid EventId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; } // to estabalish a relation between pattern and event in belongs to
        public RecurrenceFrequency? Frequency { get; set; }// to figure out what frequecy of recurrence event is
        
        

        public int? Interval { get; set; }// how frequently the event is repeated (if freq is daily and interval is 1 so it repeats daily)

        public DateTime? StartDate { get; set; } 

        public DateTime? EndDate { get; set; }// both start and end date defines the total span of an recurring event
       

    }
}

