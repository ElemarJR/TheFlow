using TheFlow.CoreConcepts;
using Xunit;

namespace TheFlow.Tests.Functional.PickingProducts
{
    public class PickingProductsProcess
    {
        public void Setup()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher<ProductOrdered>()
                .AddActivity<CheckStockActivity>()
                .AddSequenceFlow("OnProductOrdered", "CheckStock")
                ;

        }

        //public 
        
    }
}
