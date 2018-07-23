using System;
using FluentAssertions;
using TheFlow.Elements.Data;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataOutputCollectionShould
    {
        [Fact]
        public void AddDataOutputs()
        {
            var sut = new DataOutputCollection
            {
                new DataOutput("a"), "b"
            };
            sut.Count.Should().Be(2);
        }
        
        [Fact]
        public void ThrowAnInvalidOperationExceptionWhenAddingTwoOutputsWithTheSameKey()
        {
            Action sut = () => { new DataOutputCollection() {"a", "a"}; };
            sut.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowNullArgumentExceptionWhenTryingToAddNull()
        {
            Action sut = () => { new DataOutputCollection() {(DataOutput)null}; };
            sut.Should().Throw<ArgumentNullException>();
        }
    }
}