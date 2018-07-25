using System;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional
{
    public class RunningTwoPathsInParallel
    {
        [Fact]
        public void ActivitiesRanInTheRightSequence()
        {
            var op = 1;
            var e0 = 0;
            var e1 = 0;
            var e2 = 0;
            var e3 = 0;
            
            var l = new object();
            
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddActivity("msgBefore", LambdaActivity.Create(() => { e0 = op++;}))
                .AddParallelGateway("split")
                .AddSequenceFlow("start", "msgBefore", "split")
                .AddActivity("msgLeft", LambdaActivity.Create(() => { lock (l) { e1 =  op++;}}))
                .AddActivity("msgRight", LambdaActivity.Create(() => { lock (l) { e2 = op++;}}))
                .AddParallelGateway("join")
                .AddSequenceFlow("split", "msgLeft", "join")
                .AddSequenceFlow("split", "msgRight", "join")
                .AddActivity("msgAfter", LambdaActivity.Create(() => { e3 = op++;}))
                .AddEventThrower("end")
                .AddSequenceFlow("join", "msgAfter", "end");

            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);

            manager.HandleEvent(null);

            (e0 + e1 + e2 + e3).Should().Be(10);
        }
    }
}