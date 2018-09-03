using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Functional.PickingProducts
{
    public class PickingProductsProcess
    {

        public static ProcessModel CreateModel()
        {
            return ProcessModel.Create()
                .AddEventCatcher<ProductOrdered>()
                .AddActivity<CheckStockActivity>()
                .AddExclusiveGateway("InStock?")
                .AddSequenceFlow("OnProductOrdered", "CheckStock", "InStock?")

                .AddActivity<PickStockActivity>()
                .AddActivity<ShipOrderActivity>()
                .AddEventThrower<InStockEventThrower>()
                .AddSequenceFlow("PickStock", "ShipOrder", "InStock")
                .AddConditionalSequenceFlow("InStock?", "PickStock", true)

                .AddEventThrower<OutOfStockEventThrower>()
                .AddConditionalSequenceFlow("InStock?", "OutOfStock", false);
        }

        [Theory]
        [InlineData("001", false)]
        [InlineData("002", true)]
        [InlineData("003", true)]
        public void Scenario(string productCode, bool outOfStock)
        {
            InStockEventThrower.WasThrown = false;
            OutOfStockEventThrower.WasThrown = false;

            
            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(CreateModel()), 
                new InMemoryProcessInstancesStore()
                );

            manager.HandleEvent(new ProductOrdered {ProductCode = productCode});

            InStockEventThrower.WasThrown.Should().Be(!outOfStock);
            OutOfStockEventThrower.WasThrown.Should().Be(outOfStock);
        }

        //public 

    }
}
