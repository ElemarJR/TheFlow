using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Bson;
using TheFlow.CoreConcepts;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class ProcessModelShould
    {
        [Fact]
        public void HaveAnId()
        {
            ProcessModel.Create().Id
                .Should().NotBeEmpty();
        }

        
        [Fact]
        public void EnumerateStartEventCatchers()
        {
            var processModel = ProcessModel.Create()
                .AddEventCatcher(
                    "start",
                    CatchAnyEventCatcher.Instance
                )
                .AddEventThrower(
                    "end",
                    SilentEventThrower.Instance
                )
                .AddSequenceFlow("start", "end");

            var catchers = processModel.GetStartEventCatchers();
            Assert.Single(catchers);
        }

        [Fact]
        public void DifferentiateStartEventCatchersFromMiddleEventCatchers()
        {
            var processModel = ProcessModel.Create()
                .AddEventCatcher(
                    "start",
                    CatchAnyEventCatcher.Instance
                )
                .AddEventCatcher(
                    "middle",
                    CatchAnyEventCatcher.Instance
                )
                .AddEventThrower(
                    "end",
                    SilentEventThrower.Instance
                )
                .AddSequenceFlow("start", "middle")
                .AddSequenceFlow("middle", "end");

            var catchers = processModel.GetStartEventCatchers();
            Assert.Single(catchers);
        }

        [Fact]
        public void ThrowArgumentWhenInsertingElementWithDuplicatedName()
        {
            Action act = () =>
            {
                var processModel = ProcessModel.Create()
                    .AddActivity("act1", LambdaActivity.Create(() => { }))
                    .AddActivity("act1", LambdaActivity.Create(() => { }));
            };

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void EnumerateEndEventThrowers()
        {
            var processModel = ProcessModel.Create()
                .AddEventCatcher(
                    "start",
                    CatchAnyEventCatcher.Instance
                )
                .AddEventThrower(
                    "middle",
                    SilentEventThrower.Instance
                )
                .AddEventThrower(
                    "end",
                    SilentEventThrower.Instance
                )
                .AddSequenceFlow("start", "middle", "end");

            var throwers = processModel.GetEndEventThrowers();
            Assert.Single(throwers);
        }

        [Fact]
        public void ReturnElementByName()
        {
            var obj = NamedProcessElement<IEventThrower>.Create("middle", SilentEventThrower.Instance);
            var processModel = ProcessModel.Create()
                .AddEventCatcher(
                    "start",
                    CatchAnyEventCatcher.Instance
                )
                .AddEventThrower(obj)
                .AddEventThrower(
                    "end",
                    SilentEventThrower.Instance
                )
                .AddSequenceFlow("start", "middle")
                .AddSequenceFlow("middle", "end");

            processModel.GetElementByName("middle").Should().Be(obj);
        }
        
        class Start {}
        
        [Fact]
        public void AnswerIfCouldStartWithSpecificEvent()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start", TypedEventCatcher<Start>.Instance)
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "end");

            model.CanStartWith(new object()).Should().BeFalse();
            model.CanStartWith(new Start()).Should().BeTrue();

        }
        
    }
}