using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Data;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional.Basics
{
    public class DataCommunications
    {
        [Fact]
        public void UsingDataReceivedInStartEventInTheFollowingAction()
        {
            var input = "Hello World";
            string output = null;
        
            var start = CatchAnyEventCatcher.Create();
            start.SetEventDataOutput("o");
        
            var activity = LambdaActivity.Create((la, ctx) =>
            {
                var i = la.GetDataInputByName("i");
                output = (string) i.GetCurrentValue(ctx, "middle");
            });
            
            activity.Inputs.Add("i");
        
            var model = ProcessModel.Create()
                .AddEventCatcher("start", start)
                .AddActivity("middle", activity)
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "middle", "end")
                .AddDataAssociation("startToMiddle",
                    DataAssociation.Create("start", "o", "middle", "i"));
        
            var models = new InMemoryProcessModelsStore();
            models.Store(model);
            
            var instances = new InMemoryProcessInstancesStore();
            
            var pm = new ProcessManager(models, instances);
            pm.HandleEvent(input);
        
            output.Should().Be(input);
        }
    }
}
