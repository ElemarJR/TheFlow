using FluentAssertions;
using TheFlow.Elements.Events;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class CatchAnyEventCatcherShould
    {
        [Fact]
        public void HaveDefaultOutput()
        {
            var catcher = 
                (CatchAnyEventCatcher) CatchAnyEventCatcher.Create();
            catcher.GetDataOutputByName("default")
                .Should().NotBeNull();
        }
    }
}