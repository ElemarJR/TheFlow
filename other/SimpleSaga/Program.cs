using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

namespace SimpleSaga
{
    class Program
    {
        static void Main(string[] args)
        {
            var models = new InMemoryProcessModelsStore();
            var instances = new InMemoryProcessInstancesStore();

            var model = ProcessModel.Create()
                .AddEventCatcher<StartEvent>()
                .AddActivity<Activity1>()
                .AddActivity<CompensatingActivity1>()
                .AttachAsCompensationActivity("CompensatingActivity1", "Activity1")
                .AddActivity<Activity2>()
                .AddActivity<CompensatingActivity2>()
                .AttachAsCompensationActivity("CompensatingActivity2", "Activity2")
                .AddActivity<Activity3>()
                .AddEventThrower<EndEventThrower>()
                .AddSequenceFlow("OnStartEvent", 
                    "Activity1",
                    "Activity2",
                    "Activity3", 
                    "End");

            models.Store(model);

            var processManager = new ProcessManager(models, instances);

            processManager.HandleEvent(new StartEvent());
        }
    }
}
