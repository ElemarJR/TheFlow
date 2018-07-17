namespace TheFlow.Elements.Events
{
    public interface IEventCatcher : IElement
    {
        bool CanHandle(object @event);
        void Handle(object @event);
    }
}