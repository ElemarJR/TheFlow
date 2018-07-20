using System;
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
            var input = new DataInput("a");
            var output = new DataOutput("a");
            var association = DataAssociation.Create(
                input,
                output
                );

            output.Update("Hello World");
            input.CurrentValue.Should().Be("Hello World");
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenDataInputIsNull()
        {
            Action act = () =>
            {
                DataAssociation.Create(
                    null,
                    new DataOutput("a")
                );
            };
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenDataOutputIsNull()
        {
            Action act = () =>
            {
                DataAssociation.Create(
                    new DataInput("a"), 
                    null
                );
            };
            act.Should().Throw<ArgumentNullException>();
        }

    }
}