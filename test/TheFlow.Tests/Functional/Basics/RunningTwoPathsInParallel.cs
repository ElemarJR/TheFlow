using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Parallel;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional.Basics
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

            var lockObject = new object();
            
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("start")
                .AddActivity("msgBefore", () => { e0 = op++;}) // 0+1
                .AddParallelGateway("split")
                .AddSequenceFlow("start", "msgBefore", "split")
                .AddActivity("msgLeft", () => {  lock(lockObject) {e1 =  op++;}}) // 1+2
                .AddActivity("msgRight", () => {  lock(lockObject) {e2 = op++;} }) // 3+3
                .AddParallelGateway("join")
                .AddSequenceFlow("split", "msgLeft", "join")
                .AddSequenceFlow("split", "msgRight", "join")
                .AddActivity("msgAfter", () => { e3 = op++;}) // 6+4
                .AddEventThrower("end")
                .AddSequenceFlow("join", "msgAfter", "end");
        
            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();

            var sc = new ServiceCollection();
            sc.AddLogging();
            sc.AddSingleton<IProcessMonitor, InMemoryProcessMonitor>();
            var sp = sc.BuildServiceProvider();
            var manager = new ProcessManager(models, instances, sp);
        
            manager.HandleEvent(null);
        
            (e0 + e1 + e2 + e3).Should().Be(10);
        }

        [Fact]
        public void ReturnTwoTokensWhenThereAreEventHandlersInBothPaths()
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("start")
                .AddActivity("msgBefore", () => { })
                .AddParallelGateway("split")
                .AddSequenceFlow("start", "msgBefore", "split")
                .AddActivity("msgLeft", () => { })
                .AddAnyEventCatcher("evtLeft")
                .AddActivity("msgRight", () => { })
                .AddAnyEventCatcher("evtRight")
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
        
        [Fact]
        public void ReturnOneTokenWhenThereIsAnEventHandlerInOneOfThePaths()
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("start")
                .AddActivity("msgBefore", () => { })
                .AddParallelGateway("split")
                .AddSequenceFlow("start", "msgBefore", "split")
                .AddActivity("msgLeft", () => { })
                .AddAnyEventCatcher("evtLeft")
                .AddActivity("msgRight", () => { })
                .AddParallelGateway("join")
                .AddSequenceFlow("split", "msgLeft", "evtLeft", "join")
                .AddSequenceFlow("split", "msgRight", "join")
                .AddActivity("msgAfter", () => { })
                .AddEventThrower("end")
                .AddSequenceFlow("join", "msgAfter", "end");
        
            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);
        
            var results = manager.HandleEvent(null).ToArray();
            results.Length.Should().Be(1);

            var result = results[0];
            var affectedTokens = result.AffectedTokens.ToArray();
            
            affectedTokens.Length.Should().Be(1);

            result = manager
                .HandleEvent(result.ProcessInstanceId, affectedTokens[0], null);
            //.AffectedTokens.

            var instance = manager.InstancesStore.GetById(result.ProcessInstanceId);
            instance.IsDone.Should().BeTrue();
        }
    }
}