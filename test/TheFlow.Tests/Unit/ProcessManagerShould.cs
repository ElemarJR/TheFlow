using System;
using System.Linq;
using FluentAssertions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class ProcessManagerShould
    {
        class Start {}
        [Fact]
        public void CreateInstancesOnlyWhenApplicable()
        {
            var model = ProcessModel.Create()
                .AddEventCatcher("start", TypedEventCatcher<Start>.Create())
                .AddEventCatcher("middle", CatchAnyEventCatcher.Create())
                .AddEventThrower("end", SilentEventThrower.Instance)
                .AddSequenceFlow("start", "middle", "end");

            var models = new InMemoryProcessModelsStore();
            models.Store(model);
            
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);
            manager.HandleEvent(new object());
            manager.InstancesStore.GetRunningInstancesCount().Should().Be(0);
            
            manager.HandleEvent(new Start());
            manager.InstancesStore.GetRunningInstancesCount().Should().Be(1);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenTryingToAttachNullInstance()
        {
            
            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(), 
                new InMemoryProcessInstancesStore()
                );

            Action sut = () => manager.Attach(null);

            sut.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void ThrowArgumentExceptionWhenTryingToAttachInstanceWithUnrecognizedModel()
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("start")
                .AddEventThrower("end");
            
            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(), 
                new InMemoryProcessInstancesStore()
            );

            var instance = ProcessInstance.Create(model);
            
            Action sut = () => manager.Attach(instance);

            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ReturnTheProcessModelsStoreWhenRequested()
        {
            var models = new InMemoryProcessModelsStore();
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);

            manager.GetService<IProcessModelsStore>()
                .Should().Be(models);

            manager.GetService<IProcessModelProvider>()
                .Should().Be(models);
        }
        
        [Fact]
        public void ReturnTheProcessInstancesStoreWhenRequested()
        {
            var models = new InMemoryProcessModelsStore();
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);

            manager.GetService<IProcessInstancesStore>()
                .Should().Be(instances);

            manager.GetService<IProcessInstanceProvider>()
                .Should().Be(instances);
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenNonExistentInstanceIdIsInformedToHandleEvent()
        {
            var models = new InMemoryProcessModelsStore();
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);

            Action sut = () => manager.HandleEvent(Guid.NewGuid(), Guid.NewGuid(), null);
            sut.Should().Throw<InvalidOperationException>();
        }
        
        [Fact]
        public void ThrowInvalidOperationExceptionWhenNonExistentInstanceIdIsInformedToHandleActivityCompletion()
        {
            var models = new InMemoryProcessModelsStore();
            var instances = new InMemoryProcessInstancesStore();
            
            var manager = new ProcessManager(models, instances);

            Action sut = () => manager.HandleActivityCompletion(Guid.NewGuid(), Guid.NewGuid(), null);
            sut.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenNonExistentModelIdIsInformedToHandleEvent()
        {
            var model = ProcessModel.CreateWithSingleActivity(
                new UserActivity()
            );
            
            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            var manager = new ProcessManager(models, instances);
            var result = manager.HandleEvent(null).First();
            
            var manager2 = new ProcessManager(
                new InMemoryProcessModelsStore(),
                instances
                );

            Action sut = () => manager2.HandleEvent(result.ProcessInstanceId, Guid.NewGuid(), null);

            sut.Should().Throw<InvalidOperationException>();
        }
        
        [Fact]
        public void ThrowInvalidOperationExceptionWhenNonExistentModelIdIsInformedToHandleActivityCompletion()
        {
            var model = ProcessModel.CreateWithSingleActivity(
                new UserActivity()
            );
            
            var models = new InMemoryProcessModelsStore(model);
            var instances = new InMemoryProcessInstancesStore();
            var manager = new ProcessManager(models, instances);
            var result = manager.HandleEvent(null).First();
            
            var manager2 = new ProcessManager(
                new InMemoryProcessModelsStore(),
                instances
            );

            Action sut = () => manager2.HandleActivityCompletion(result.ProcessInstanceId, Guid.NewGuid(), null);

            sut.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void HandlingAnEventThatContinuesWithAnotherEventReturnsAnToken()
        {
            var model = ProcessModel.Create()
                .AddAnyEventCatcher("OnStart")
                .AddAnyEventCatcher("OnEvt1")
                .AddAnyEventCatcher("OnEvt2")
                .AddEventThrower("End")
                .AddSequenceFlow("OnStart", "OnEvt1", "OnEvt2", "End");

            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(model),
                new InMemoryProcessInstancesStore()
                );

            var e = manager.HandleEvent(null).First();

            var result = manager.HandleEvent(
                e.ProcessInstanceId, e.AffectedTokens.First(), null
                );

            result.AffectedTokens.Should().ContainInOrder(e.AffectedTokens);
            result.ProcessInstanceId.Should().Be(e.ProcessInstanceId);
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenTryingToHandleFailureOfNonExistentInstance()
        {
            var manager = new ProcessManager(
                new InMemoryProcessModelsStore(),
                new InMemoryProcessInstancesStore() 
                );

            Action a = () => manager.HandleActivityFailure(Guid.NewGuid(), Guid.Empty, null);
            a.Should().Throw<InvalidOperationException>();
        }
    }
}