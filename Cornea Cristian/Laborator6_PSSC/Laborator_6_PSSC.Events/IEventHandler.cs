
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Laborator_6_PSSC.Events.Models;

namespace Laborator_6_PSSC.Events
{
    public interface IEventHandler
    {
        string[] EventTypes { get; }

        Task<EventProcessingResult> HandleAsync(CloudEvent cloudEvent);
    }
}
