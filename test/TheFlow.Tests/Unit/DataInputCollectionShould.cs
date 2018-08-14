using System;
using FluentAssertions;
using TheFlow.Elements.Data;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataInputCollectionShould
    {
        [Fact]
        public void HaveDefault()
        {
            var sut = new DataInputCollection();
            sut["default"].Should().NotBeNull();
        }
        
        [Fact]
        public void AddDataInputs()
        {
            var sut = new DataInputCollection
            {
                new DataInput("a"), "b"
            };
            sut.Count.Should().Be(3);
        }
        
        [Fact]
        public void ThrowAnInvalidOperationExceptionWhenAddingTwoInputsWithTheSameKey()
        {
            Action sut = () => { new DataInputCollection {"a", "a"}; };
            sut.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowNullArgumentExceptionWhenTryingToAddNull()
        {
            Action sut = () => { new DataInputCollection() {(DataInput)null}; };
            sut.Should().Throw<ArgumentNullException>();
        }
    }
}