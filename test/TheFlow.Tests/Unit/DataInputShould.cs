using System;
using System.Reflection.PortableExecutable;
using FluentAssertions;
using TheFlow.Elements.Data;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataInputShould
    {
        [Fact]
        public void HoldValueAfterUpdate()
        {
            var di = new DataInput("a");
            di.Update("Hello World");
            di.CurrentValue.Should().Be("Hello World");
        }
        
    }
}