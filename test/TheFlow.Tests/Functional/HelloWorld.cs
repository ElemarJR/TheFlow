using System;
using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;
using Xunit;

namespace TheFlow.Tests.Functional
{
    public class HelloWorld
    {
        [Fact]
        public void Event_Activity_End()
        {
            var passed = false;
            var model = ProcessModel.Create()
                .AddEventCatcher("start", CatchAnyEventCatcher.Create())
                .AddActivity("activity", LambdaActivity.Create(() => passed = true))
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "activity", "end");

            var instance = ProcessInstance.Create(model.Id);
            
            var context = new ExecutionContext(null, model, instance, instance.Token, null);
            instance.HandleEvent(context, new object());
            passed.Should().BeTrue();
        }

        [Fact]
        public void CreateWithSingleActivity()
        {
            var passed = false;
            var model = ProcessModel.CreateWithSingleActivity(
                    LambdaActivity.Create(() => passed = true)
                    );

            var instance = ProcessInstance.Create(model);

            var context = new ExecutionContext(null, model, instance, instance.Token, null);
            instance.HandleEvent(context, new object());
            passed.Should().BeTrue();
        }
    }
}