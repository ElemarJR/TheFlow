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
                    CatchAnyEventCatcher.Create()
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
                    CatchAnyEventCatcher.Create()
                )
                .AddEventCatcher(
                    "middle",
                    CatchAnyEventCatcher.Create()
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
                    CatchAnyEventCatcher.Create()
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
                    CatchAnyEventCatcher.Create()
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
                .AddEventCatcher("start", TypedEventCatcher<Start>.Create())
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "end");

            model.CanStartWith(new object()).Should().BeFalse();
            model.CanStartWith(new Start()).Should().BeTrue();

        }

        [Fact]
        public void AddEventCatcherShouldCreateDefaultHandlerWhenItIsNotSpecified()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start");

            var element = model.GetElementByName("start")?.Element;
            element.Should().BeOfType<CatchAnyEventCatcher>();
        }

        [Fact]
        public void AddEventThrowerShouldCreateDefaultThrowerWhenItIsNotSpecified()
        {
            var model = ProcessModel.Create()
                .AddEventThrower("end");

            var element = model.GetElementByName("end")?.Element;
            element.Should().BeOfType<SilentEventThrower>();
        }
        
    }
}