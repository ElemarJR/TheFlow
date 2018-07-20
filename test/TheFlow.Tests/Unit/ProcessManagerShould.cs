using System;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class ProcessManagerShould
    {
        class Start {}
        [Fact]
        public void CreateInstancesOnlyWhenApplicable()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start", TypedEventCatcher<Start>.Instance)
                .AddEventCatcher("middle", CatchAnyEventCatcher.Create())
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "middle", "end");

            var models = new InMemoryProcessModelsStore();
            models.Store(model);
            
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);
            manager.HandleEvent(new object());
            manager.InstancesStore.GetRunningInstancesCount().Should().Be(0);
            
            manager.HandleEvent(new Start());
            manager.InstancesStore.GetRunningInstancesCount().Should().Be(1);
        }
        
        
        
    }
}