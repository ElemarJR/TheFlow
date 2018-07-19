using System;
using FluentAssertions;
using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;

using Xunit;

using static System.Console;

namespace TheFlow.Tests.Functional
{
    public class UsingDataInputs
    {
        [Fact]
        public void UsingDataReceivedInStartEventInTheFollowingAction()
        {

        }
        //[Theory]
        //public void UsingDataReceivedInStartEventInTheFollowingAction()
        //{
        //    //string input = "Hello World";
        //    //string output = null;
        //    //var model = ProcessModel.Create(Guid.NewGuid())
        //    //    .AddEventCatcher("start", CatchAnyEventCatcher.Builder()
        //    //        .AddDataOutput("eventData")
        //    //        .Build()
        //    //    )
        //    //    .AddActivity("middle", LambdaActivity.Create((sp) =>
        //    //        {
        //    //            var dataSource = sp.GetService<IDataSource>();
        //    //            output = (string)dataSource.GetData("startEventData");
        //    //        }).AddDataInput("startEventData")
        //    //    )
        //    //    .AddEventThrower("end", SilentEventThrower.Instance)
        //    //    .AddSequenceFlow("start", "middle", "end")
        //    //    .AddDataAssociation(
        //    //        fromElement: "start",
        //    //        fromDataOutput: "eventData",
        //    //        toElement: "middle",
        //    //        toDataInput: "startEventData"
        //    //        );

        //    //var instance = ProcessInstance.Create(model);

        //    //instance.HandleEvent(instance.Token.Id, model, input);

        //    //output.Should().Be(input);
        //}
    }
}
