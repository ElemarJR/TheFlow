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
        public void BasicSetup()
        {
            var passed = false;
            var model = ProcessModel.Create()
                .AddEventCatcher("start", CatchAnyEventCatcher.Instance)
                .AddActivity("activity", LambdaActivity.Create(() => passed = true))
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "activity", "end");

            var instance = ProcessInstance.Create(model.Id);

            instance.HandleEvent(model, new object());
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

            instance.HandleEvent(model, new object());
            passed.Should().BeTrue();
        }
    }
}