using System;
using FluentAssertions;
using TheFlow.Elements.Data;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataOutputShould
    {
        [Fact]
        public void ThrowArgumentNullExceptionWhenNameIsNull()
        {
            Action sut = () => new DataOutput(null);
            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentExceptionWhenNameIsEmpty()
        {
            Action sut = () => new DataOutput(string.Empty);
            sut.Should().Throw<ArgumentException>();

        }
    }
}