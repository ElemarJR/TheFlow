using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional
{
    public class MakingSimpleDecisions
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SimpleDecisionMakingProcess(bool data)
        {
            var output = false;
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddExclusiveGateway("If")
                .AddActivity("True", () => output = true)
                .AddActivity("False", () => output = false)
                .AddExclusiveGateway("EndIf")
                .AddEventThrower("end")
                .AddSequenceFlow("start", "If")
                .AddConditionalSequenceFlow("If", "True", true)
                .AddConditionalSequenceFlow("If", "False", false)
                .AddSequenceFlow("True", "EndIf")
                .AddSequenceFlow("False", "EndIf")
                .AddSequenceFlow("EndIf", "end");

            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);

            manager.HandleEvent(data);

            output.Should().Be(data);
        }
    }
}