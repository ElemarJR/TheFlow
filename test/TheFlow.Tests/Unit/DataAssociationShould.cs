using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;
using TheFlow.Infrastructure.Stores;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class DataAssociationShould
    {
        [Fact]
        public void UpdateTheDataInputCurrentValue()
        {
            var output = new DataOutput("a");
            var element1 = new NullElement();
            element1.Outputs.Add(output);
            
            var input = new DataInput("a");
            var element2 = new NullElement();
            element2.Inputs.Add(input);
            
            var model = ProcessModel.Create()
                .AddNullElement("element1", element1)
                .AddNullElement("element2", element2)
                .AddDataAssociation("assoc", DataAssociation.Create("element1", "a", "element2", "a"));
            
            var models = new InMemoryProcessModelsStore();
            models.Store(model); 

            var manager = new ProcessManager(
                models, 
                new InMemoryProcessInstancesStore()
                );
            
            var instance = ProcessInstance.Create(model);
            manager.Attach(instance);
            
            var context = new ExecutionContext(manager, model, instance, instance.Token, null);
            
            output.Update(context, "element1", "Hello World");
            input.GetCurrentValue(context, "element2").Should().Be("Hello World");
        }
    }
}