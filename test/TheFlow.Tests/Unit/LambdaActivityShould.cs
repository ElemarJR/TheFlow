using System;
using FluentAssertions;
using TheFlow.Elements.Activities;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class LambdaActivityShould
    {
        [Fact]
        public void ThrowArgumentNullExceptionWhenNoLambdaIsSpecified()
        {
            Action act = () => { LambdaActivity.Create((Action<IServiceProvider>) null); };
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
