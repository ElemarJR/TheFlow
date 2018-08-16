using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

public static class Program
{
    public static void Main()
    {
        var e1 = false;
        var e2 = false;
        var e3 = false;

        var model = ProcessModel.Create()
            .AddEventCatcher("start")
            .AddActivity("a1", () => { })
            .AddActivity("c1", () => e1 = true)
            .AttachAsCompensationActivity("c1", "a1")
            .AddActivity("a2", () => throw new Exception())
            .AddActivity("c2", () => e2 = true)
            .AttachAsCompensationActivity("c2", "a2")
            .AddActivity("a3", () => { })
            .AddActivity("c3", () => e3 = true)
            .AttachAsCompensationActivity("c3", "a3")
            .AddEventThrower("end")
            .AddSequenceFlow("start", "a1", "a2", "a3", "end");

        var models = new InMemoryProcessModelsStore(model);
        var instances = new InMemoryProcessInstancesStore();

        var sc = new ServiceCollection();
        sc.AddLogging(cfg => { cfg.AddConsole(); });

        var manager = new ProcessManager(models, instances, sc.BuildServiceProvider());
        var result = manager.HandleEvent(null).First();

        var relatedInstance = manager.InstancesStore.GetById(result.ProcessInstanceId);
        
    }
}