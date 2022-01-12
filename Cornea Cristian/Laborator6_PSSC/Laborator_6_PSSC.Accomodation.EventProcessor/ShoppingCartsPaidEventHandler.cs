using Laborator_6_PSSC.Events;
using Laborator_6_PSSC.Dto.Events;
using Laborator_6_PSSC.Events.Models;
using System;
using System.Threading.Tasks;

namespace Laborator_6_PSSC.Accomodation.EventProcessor
{
    internal class ShoppingCartsPaidEventHandler : AbstractEventHandler<ShoppingCartsPublishEvent>
    {
        public override string[] EventTypes => new string[]{typeof(ShoppingCartsPublishEvent).Name};

        protected override Task<EventProcessingResult> OnHandleAsync(ShoppingCartsPublishEvent eventData)
        {
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}
