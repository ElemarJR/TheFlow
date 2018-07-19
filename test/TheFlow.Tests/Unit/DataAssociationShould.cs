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
            var input = new DataInput<string>("a");
            var output = new DataOutput<string>("a");
            var association = DataAssociation<string>.Create(
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
                DataAssociation<string>.Create(
                    null,
                    new DataOutput<string>("a")
                );
            };
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenDataOutputIsNull()
        {
            Action act = () =>
            {
                DataAssociation<string>.Create(
                    new DataInput<string>("a"), 
                    null
                );
            };
            act.Should().Throw<ArgumentNullException>();
        }

    }
}