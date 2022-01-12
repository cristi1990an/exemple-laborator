using LanguageExt;

namespace Laborator_6_PSSC.Events
{
    public interface IEventSender
    {
        TryAsync<Unit> SendAsync<T>(string topicName, T @event);
    }
}
