using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure.Stores;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
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

            var sc = new ServiceCollection();
            sc.AddLogging(config =>
            {
                config.AddConsole(options => options.IncludeScopes = true);
                //config
            });
            
            
            var manager = new ProcessManager(models, instances, sc);

            manager.HandleEvent(null);
        }
    }
}
