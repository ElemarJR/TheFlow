using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class CatchAnyEventCatcherShould
    {
        [Fact]
        public void HaveDefaultOutput()
        {
            var catcher = 
                (CatchAnyEventCatcher) CatchAnyEventCatcher.Create();
            catcher.GetDataOutputByName("default")
                .Should().NotBeNull();
        }

        [Theory]
        [InlineData("Hello World")]
        public void SaveEventDataAsDataObject(object data)
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("OnMessageReceived")
                .AddEventThrower("GoodBye", SilentEventThrower.Instance)
                .AddSequenceFlow("OnMessageReceived", "GoodBye");

            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(model),
                new InMemoryProcessInstancesStore()
            );

            var result = manager.HandleEvent(data);
            var pinstance = manager.InstancesStore.GetById(result.First().ProcessInstanceId);

            pinstance.GetDataObjectValue("MessageReceived").Should().Be(data);
        }
    }
}