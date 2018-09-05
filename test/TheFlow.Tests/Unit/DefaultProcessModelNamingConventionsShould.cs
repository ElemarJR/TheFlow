using FluentAssertions;
using TheFlow.Conventions;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DefaultProcessModelNamingConventionsShould
    {
        [Theory]
        [InlineData("OnDataObject", "DataObject")]
        [InlineData("onDataObject", "DataObject")]
        [InlineData("OneInfo", "OneInfo")]
        [InlineData("oneInfo", "oneInfo")]
        public void ForDataObjects_NotStartWith_On_(string original, string expected)
        {
            new ProcessModelNamingConventions()
                .DataObjectName(original).Should().Be(expected);
        }

        [Theory]
        [InlineData("FooActivity", "Foo")]
        [InlineData("Inactivity", "Inactivity")]
        public void ForAcitivities_NotHaveTheSuffix_Activity(string original, string expected)
        {
            new ProcessModelNamingConventions().ActivityName(original).Should().Be(expected);
        }

        [Theory]
        [InlineData("FooEventThrower", "Foo")]
        [InlineData("TheEvent", "TheEvent")]
        public void ForEventThrowers_NotHaveTheSuffix_EventThrower(string original, string expected)
        {
            new ProcessModelNamingConventions().EventThrower(original).Should().Be(expected);
        }

        [Theory]
        [InlineData("OnEvent", "OnEvent")]
        [InlineData("onEvent", "onEvent")]
        [InlineData("Event", "OnEvent")]
        public void ForEventCatchers_ShouldStartWith_On(string original, string expected)
        {
            new ProcessModelNamingConventions().EventCatcher(original).Should().Be(expected);
        }
    }
}
