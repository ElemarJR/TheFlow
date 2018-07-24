using System;
using TheFlow.Elements;

namespace TheFlow.CoreConcepts
{
    public class ExecutionContext
    {
        public IServiceProvider ServiceProvider { get; }
        public ProcessModel Model { get; }
        public ProcessInstance Instance { get; }
        public Token Token { get; }
        public IElement RunningElement { get; }

        public ExecutionContext(
            IServiceProvider serviceProvider, 
            ProcessModel model, 
            ProcessInstance instance, Token token, 
            IElement runningElement)
        {
            ServiceProvider = serviceProvider;
            Model = model;
            Instance = instance;
            Token = token;
            RunningElement = runningElement;
        }
    }
}