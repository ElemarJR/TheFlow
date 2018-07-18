using FluentAssertions;
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
            var association = DataAssociation<string>.Create(
                input,
                output
                );

            output.Update("Hello World");
            input.CurrentValue.Should().Be("Hello World");
        }
       
    }
}