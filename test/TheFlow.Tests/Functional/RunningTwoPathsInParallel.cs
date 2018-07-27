using System;
using System.Linq;
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
            
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddActivity("msgBefore", () => { e0 = op++;})
                .AddParallelGateway("split")
                .AddSequenceFlow("start", "msgBefore", "split")
                .AddActivity("msgLeft", () => {  e1 =  op++;})
                .AddActivity("msgRight", () => {  e2 = op++; })
                .AddParallelGateway("join")
                .AddSequenceFlow("split", "msgLeft", "join")
                .AddSequenceFlow("split", "msgRight", "join")
                .AddActivity("msgAfter", () => { e3 = op++;})
                .AddEventThrower("end")
                .AddSequenceFlow("join", "msgAfter", "end");
        
            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);
        
            manager.HandleEvent(null);
        
            (e0 + e1 + e2 + e3).Should().Be(10);
        }

        [Fact]
        public void ReturnTwoTokensWhenThereIsAnEventHandlerInOneOfThePaths()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddActivity("msgBefore", () => { })
                .AddParallelGateway("split")
                .AddSequenceFlow("start", "msgBefore", "split")
                .AddActivity("msgLeft", () => { })
                .AddEventCatcher("evtLeft")
                .AddActivity("msgRight", () => { })
                .AddEventCatcher("evtRight")
                .AddParallelGateway("join")
                .AddSequenceFlow("split", "msgLeft", "evtLeft", "join")
                .AddSequenceFlow("split", "msgRight", "evtRight", "join")
                .AddActivity("msgAfter", () => { })
                .AddEventThrower("end")
                .AddSequenceFlow("join", "msgAfter", "end");
        
            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);
        
            var result = manager.HandleEvent(null);
            result.FirstOrDefault().AffectedTokens.Count().Should().Be(2);
        }
    }
}