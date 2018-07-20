using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional
{
    public class BasicFlowBasedOnEvents
    {
        class Start {}

        [Fact]
        public void StartMiddleEnd()
        {
            var models = new InMemoryProcessModelsStore();
            models.Store(ProcessModel.Create()
                .AddEventCatcher("start", TypedEventCatcher<Start>.Instance)
                .AddEventCatcher("middle", CatchAnyEventCatcher.Create())
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "middle", "end"));

            var instances = new InMemoryProcessInstancesStore();

            var manager = new ProcessManager(models, instances);

            var result = manager.HandleEvent(new Start()).First();
            manager.GetExecutionPoint(
                result.ProcessInstanceId,
                result.AffectedTokens.First()
            ).Should().Be("middle");

            result = manager.HandleEvent(result.ProcessInstanceId, result.AffectedTokens.First(), new object());
            manager.InstancesStore.GetById(result.ProcessInstanceId)
                .IsDone.Should().Be(true);
        }
    }
}