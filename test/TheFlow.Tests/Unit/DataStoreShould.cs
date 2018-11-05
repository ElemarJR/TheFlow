using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataStoreShould
    {
        [Theory]
        [InlineData("v1", "v2")]
        public void StoreDifferentValuesPerInstance(
            string valueForInstance1, 
            string valueForInstance2
            )
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("start")
                .AddEventThrower("end")
                .AddSequenceFlow("start", "end")
                .AddDataStore<string>("data");

            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(model),
                new InMemoryProcessInstancesStore()
                );

            var r1 = manager.HandleEvent(null).First();
            var r2 = manager.HandleEvent(null).First();
            
            var instance1 = manager.InstancesStore.GetById(
                r1.ProcessInstanceId
            );

            var instance2 = manager.InstancesStore.GetById(
                r2.ProcessInstanceId
            );

            var dataStore = model.GetElementByName("data").Element as IDataStore<string>;

            var ec1 = new ExecutionContext(null, model, instance1, null, null);
            var ec2 = new ExecutionContext(null, model, instance2, null, null);

            dataStore.SetValue(ec1, valueForInstance1);
            dataStore.SetValue(ec2, valueForInstance2);

            dataStore.GetValue(ec1).Should().Be(valueForInstance1);
            dataStore.GetValue(ec2).Should().Be(valueForInstance2);
        }

    }
}
