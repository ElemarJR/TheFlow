using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace TheFlow.Tests.Functional.PickingProducts
{
    public class CheckStockActivity : Activity
    {
        public override void Run(ExecutionContext context)
        {
            var productOrdered = (ProductOrdered) context.Instance
                .GetDataObjectValue("ProductOrdered");

            var result = productOrdered.ProductCode == "001";

            SetDefaultOuputTo(context, result);
            context.Instance.HandleActivityCompletion(context, result);
        }
    }
}