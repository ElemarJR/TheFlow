using System;
using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class ProcessInstanceShould
    {
        [Fact]
        public void HaveAToken()
        {
            var instance = ProcessInstance.Create(Guid.NewGuid());
            instance.Token.Should().NotBeNull();
        }

        [Fact]
        public void HandleStartEventProperly()
        {
            
            var model = ProcessModel.Create()
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
            var instance = ProcessInstance.Create(Guid.Parse(model.Id));
            instance.HandleEvent(
                instance.Token.Id, model, new object()
                );
            instance.Token.ExecutionPoint.Should().Be("middle");
        }


        [Fact]
        public void SaveStartEventNameAtTheFirstHistoryEntry()
        {

            var model = ProcessModel.Create()
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

            var instance = ProcessInstance.Create(Guid.Parse(model.Id));
            instance.HandleEvent(
                instance.Token.Id, model, new object()
            );
            
            instance.History.First().ExecutionPoint.Should().Be("start");
        }

        [Fact]
        public void BeNotRunnuingBeforeHandlingValidStartEvent()
        {
            var instance = ProcessInstance.Create(Guid.NewGuid());
            instance.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void BeRunningAfterHandlingValidStartEvent()
        {
            
            var model = ProcessModel.Create()
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
                .AddSequenceFlow("start", "middle", "end");

            var instance = ProcessInstance.Create(Guid.Parse(model.Id));
            instance.HandleEvent(
                instance.Token.Id, model, new object()
                );
            instance.IsRunning.Should().BeTrue();
            instance.Token.ExecutionPoint.Should().Be("middle");
        }

        [Fact]
        public void ReturnTwoTokensWhenStartEventHaveTwoOutcomingConnections()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher(
                    "start",
                    CatchAnyEventCatcher.Create()
                )
                .AddEventCatcher(
                    "middle1", 
                    CatchAnyEventCatcher.Create()
                )
                .AddEventCatcher(
                    "middle2",
                    CatchAnyEventCatcher.Create()
                )
                .AddEventThrower(
                    "end",
                    SilentEventThrower.Instance
                )
                .AddSequenceFlow("start", "middle1")
                .AddSequenceFlow("start", "middle2")
                .AddSequenceFlow("middle1", "end")
                .AddSequenceFlow("middle2", "end");

            var instance = ProcessInstance.Create(model.Id);
            var tokens = instance.HandleEvent(instance.Token.Id, model, new object());

            tokens.Count().Should().Be(2);

            tokens.Select(token => token.ExecutionPoint)
                .Should().BeEquivalentTo("middle1", "middle2");
        }
        
        [Fact]
        public void FinishWhenMeetingTheEnd()
        {
            
            var model = ProcessModel.Create()
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
            
            var instance = ProcessInstance.Create(model.Id);
            
            // start
            instance.HandleEvent(
                instance.Token.Id, model, new object()
            ).Should().BeEquivalentTo(instance.Token);
                
            // middle
            instance.HandleEvent(
                instance.Token.Id, model, new object()
            ).Should().BeEquivalentTo(instance.Token);
                
            instance.IsRunning.Should().BeFalse();
            instance.IsDone.Should().BeTrue();
        }

        [Fact]
        public void ExecuteActivitiesBetweenEvents()
        {
            var count = 0;
            var model = ProcessModel.Create()
                .AddEventCatcher("start", CatchAnyEventCatcher.Create())
                .AddActivity("a1", LambdaActivity.Create(() => count++))
                .AddActivity("a2", LambdaActivity.Create(() => count++))
                .AddActivity("a3", LambdaActivity.Create(() => count++))
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "a1", "a2", "a3", "end");
                
            var instance = ProcessInstance.Create(model.Id);
            instance.HandleEvent(model, new object());

            instance.IsDone.Should().BeTrue();
            instance.IsRunning.Should().BeFalse();
            instance.History.Count().Should().Be(8);
            count.Should().Be(3);
        }

        [Fact]
        public void RefuseEventsWhenItIsDone()
        {
            var count = 0;
            var model = ProcessModel.Create()
                .AddEventCatcher("start", CatchAnyEventCatcher.Create())
                .AddActivity("a1", LambdaActivity.Create(() => count++))
                .AddActivity("a2", LambdaActivity.Create(() => count++))
                .AddActivity("a3", LambdaActivity.Create(() => count++))
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "a1", "a2", "a3", "end");
            
            var instance = ProcessInstance.Create(model.Id);
            
            instance.HandleEvent(model, new object());
            instance.HandleEvent(model, new object());
            
            count.Should().Be(3);
        }

        [Fact]
        public void TokenShouldNotBeActiveWhenProcessIsDone()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start", CatchAnyEventCatcher.Create())
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "end");

            var instance = ProcessInstance.Create(model.Id);
            instance.HandleEvent(model, new object());

            instance.Token.WasReleased.Should().BeTrue();
            instance.Token.IsActive.Should().BeFalse();

        }

        class Start {}
        class Middle1 {}
        class Middle2 {}

        [Fact]
        public void RespectEventTypes()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start", TypedEventCatcher<Start>.Create())
                .AddEventCatcher("middle1", TypedEventCatcher<Middle1>.Create())
                .AddEventCatcher("middle2", TypedEventCatcher<Middle2>.Create())
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "middle1", "middle2", "end");

            var processInstance = ProcessInstance.Create(model.Id);

            // nothing happens
            processInstance.HandleEvent(model, new object());
            processInstance.Token.ExecutionPoint.Should().BeNull();

            processInstance.HandleEvent(model, new Start());
            processInstance.Token.ExecutionPoint.Should().Be("middle1");
            processInstance.HandleEvent(model, new Start());
            processInstance.Token.ExecutionPoint.Should().Be("middle1");
            
            processInstance.HandleEvent(model, new Middle2());
            processInstance.Token.ExecutionPoint.Should().Be("middle1");
            
            processInstance.HandleEvent(model, new Middle1());
            processInstance.Token.ExecutionPoint.Should().Be("middle2");

            processInstance.HandleEvent(model, new Middle1());
            processInstance.Token.ExecutionPoint.Should().Be("middle2");

            processInstance.HandleEvent(model, new Middle2());
            processInstance.Token.ExecutionPoint.Should().BeNull();

            processInstance.HandleEvent(model, new Middle2());
            processInstance.Token.ExecutionPoint.Should().BeNull();

            processInstance.History.Count().Should().Be(4);
            processInstance.IsDone.Should().BeTrue();
            processInstance.Token.WasReleased.Should().BeTrue();
        }
        
    }
}