using EventScheduler.Models;
using MediatR;

namespace EventScheduler.Data.Command
{
   
        public class DeleteEventCommand : IRequest<bool>
        {
            public string EventTitle { get; set; }

            public DeleteEventCommand(string eventTitle)
            {
                EventTitle = eventTitle;
            }
        }
    }


