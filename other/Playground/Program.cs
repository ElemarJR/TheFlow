using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

public static class Program
{
    public static void Main()
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

        var sc = new ServiceCollection();
        sc.AddLogging(cfg => { cfg.AddConsole(); });
            
        var manager = new ProcessManager(models, instances, sc.BuildServiceProvider());

        manager.HandleEvent(true);
    }
}