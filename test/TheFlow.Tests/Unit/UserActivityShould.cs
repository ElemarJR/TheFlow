using System;
using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class UserActivityShould
    {
        [Fact]
        public void PauseTheExecutionOfTheProcess()
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("start")
                .AddActivity("activity", new UserActivity())
                .AddEventThrower("end")
                .AddSequenceFlow("start", "activity", "end");
            
            var instances = new InMemoryProcessInstancesStore();
            var models = new InMemoryProcessModelsStore(model);
            
            var manager = new ProcessManager(models, instances);

            var result = manager.HandleEvent(null).FirstOrDefault();
            var instance = manager
                .InstancesStore
                .GetById(result.ProcessInstanceId);
            
            instance.IsDone.Should().BeFalse();


            result = manager.HandleActivityCompletion(
                result.ProcessInstanceId,
                result.AffectedTokens.First(),
                null
            );

            instance.IsDone.Should().BeTrue();
        }
    }
}