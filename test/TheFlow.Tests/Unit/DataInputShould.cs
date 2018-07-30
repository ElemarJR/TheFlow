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
        public void ThrowArgumentNullExceptionWhenNameIsNull()
        {
            Action sut = () => new DataInput(null);
            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentExceptionWhenNameIsEmpty()
        {
            Action sut = () => new DataInput(string.Empty);
            sut.Should().Throw<ArgumentException>();

        }
    }
}