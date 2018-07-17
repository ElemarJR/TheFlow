using TheFlow.Elements.Data;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataAssociationShould
    {
        [Fact]
        public void UpdateTheDataInputCurrentValue()
        {
            var input = new DataInput<string>("a");
            var output = new DataOutput<string>("a");
        }
       
    }
}