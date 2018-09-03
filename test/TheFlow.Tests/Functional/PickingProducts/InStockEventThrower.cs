using TheFlow.CoreConcepts;
using TheFlow.Elements.Events;

namespace TheFlow.Tests.Functional.PickingProducts
{
    public class InStockEventThrower : IEventThrower
    {
        public static bool WasThrown { get; set; }
        public void Throw(ExecutionContext context)
        {
            WasThrown = true;
        }
    }
}