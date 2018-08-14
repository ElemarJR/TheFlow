using System;
using System.Data;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional
{
    public class Transactions
    {
        [Fact]
        public void WhenTheProcessFailsCompensationActivitiesRun()
        {
            var data = 0;
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddActivity("regular", () => data = 10)
                .AddActivity("compensation", () => data -= 5)
                .AttachAsCompensationActivity("compensation", "regular")
                .AddActivity("failing", () => throw new Exception())
                .AddEventCatcher("end")
                .AddSequenceFlow("start", "regular", "failing", "end");

            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();

            var manager = new ProcessManager(models, instances);

            manager.HandleEvent(null);
            data.Should().Be(5);
        }

    }
}
