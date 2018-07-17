using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Events;
using TheFlow.Notifications.Application.Workflow.Events;

namespace TheFlow.Notifications.Application.Workflow
{
    static class MessagingProcessFactory
    {

        public static readonly Guid MessagingProcessId =
            Guid.Parse("368edbc0-97c1-4d92-a0e6-fd82c25fe1a8");

        public static ProcessModel Create() => ProcessModel.Create(MessagingProcessId)
            .AddEventCatcher("newMessage", TypedEventCatcher<NewMessageEvent>.Instance)
            .AddEventCatcher("processed", TypedEventCatcher<MessageProcessedEvent>.Instance)
            .AddEventThrower("end", SilentEventThrower.Instance)
            .AddSequenceFlow("newMessage", "processed", "end");
    }
}