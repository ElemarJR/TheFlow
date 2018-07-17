using FluentAssertions;
using TheFlow.Elements.Data;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataOutputShould
    {
        [Fact]
        public void AcceptSubscription()
        {
            string value = null;
            var output = new DataOutput<string>("a");
            output.Subscribe((s) => value = s);
            output.Update("Hello World");
            value.Should().Be("Hello World");
        }
    }
}