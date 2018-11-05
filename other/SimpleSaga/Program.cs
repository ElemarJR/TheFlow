using System;
using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

namespace SimpleSaga
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = ProcessModel.Create(Guid.Parse("a12637a3-72de-4774-b60c-d98310438c26"))
                .AddEventCatcherFor<StartEvent>()
                .AddActivity<Activity1>()
                .AddActivity<CompensatoryActivity1>()
                .AttachAsCompensationActivity("CompensatingActivity1", "Activity1")
                .AddActivity<Activity2>()
                .AddActivity<CompensatoryActivity2>()
                .AttachAsCompensationActivity("CompensatingActivity2", "Activity2")
                .AddActivity<Activity3>()
                .AddEventThrower<EndEventThrower>()
                .AddSequenceFlow("OnStartEvent", 
                    "Activity1",
                    "Activity2",
                    "Activity3", 
                    "End");

            ProcessManagerHolder.Instance.ModelsStore.Store(model);

            ProcessManagerHolder.Instance.HandleEvent(new StartEvent());
        }
    }
}
