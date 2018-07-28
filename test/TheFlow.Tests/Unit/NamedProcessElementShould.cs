using System;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class NamedProcessElementShould
    {
        [Fact]
        public void ThrowArgumentNullExceptionWhenNameIsNull()
        {
            Action sut = () => NamedProcessElement<NullElement>.Create(null, new NullElement());
            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenElementIsNull()
        {
            Action sut = () => NamedProcessElement<NullElement>.Create("a", null);
        }
    }
}