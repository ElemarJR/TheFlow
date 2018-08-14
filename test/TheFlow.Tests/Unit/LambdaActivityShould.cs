using System;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class LambdaActivityShould
    {
        [Fact]
        public void ThrowArgumentNullExceptionWhenNoLambdaIsSpecified()
        {
            Action act = () => { LambdaActivity.Create((Action<ExecutionContext>) null); };
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
