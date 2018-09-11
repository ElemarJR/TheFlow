using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Validations;
using Xunit;

namespace TheFlow.Tests.Unit.Validations
{
    public class EnsureThereAreOutcomingConnectionsWhenNecessaryValidationRuleShould
    {
        [Fact]
        public void AcceptModelsWhenAllActivitiesHaveOutcomingConnections()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddActivity("none", () => { })
                .AddEventThrower("end")
                .AddSequenceFlow("start",  "none" , "end");

            new EnsureThereAreOutcomingConnectionsWhenNecessaryValidationRule()
                .Validate(model).Should().BeEmpty();
        }

        [Theory]
        [InlineData("none")]
        [InlineData("ab")]
        public void RejectModelsWhenOneActivityHasNoOutcomingConnections(string activityName)
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start")
                .AddActivity(activityName, () => { })
                .AddEventThrower("end")
                .AddSequenceFlow("start", "end");

            var result = new EnsureThereAreOutcomingConnectionsWhenNecessaryValidationRule()
                .Validate(model).ToArray();

            result.Should().NotBeEmpty();
            result.First().Message.Should().Be($"'{activityName}' has no outgoing connections.");
        }

        [Theory]
        [InlineData("none")]
        [InlineData("ab")]
        public void RejectModelsWhenOneEventCatcherHasNoOutcomingConnections(string catcherName)
        {
            var model = ProcessModel.Create()
                .AddEventCatcher(catcherName)
                .AddEventThrower("end")
                .AddSequenceFlow("start", "end");

            var result = new EnsureThereAreOutcomingConnectionsWhenNecessaryValidationRule()
                .Validate(model).ToArray();

            result.Should().NotBeEmpty();
            result.First().Message.Should().Be($"'{catcherName}' has no outgoing connections.");
        }
    }
}
