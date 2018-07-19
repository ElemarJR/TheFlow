using System;
using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Activities
{
    public class LambdaActivity : Activity
    {
        public Action<IServiceProvider> Action { get; }

        private LambdaActivity(Action<IServiceProvider> action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }
       
        public static LambdaActivity Create(
            Action action
        ) => Create(sp => action());

        public static LambdaActivity Create(
            Action<IServiceProvider> action
        ) => new LambdaActivity(action);

        public override void Run(
            IServiceProvider serviceProvider,
            Guid instanceId, 
            Guid tokenId
            )
        {
            var instance = serviceProvider
                .GetService<IProcessInstanceProvider>()
                .GetProcessInstance(instanceId);
            
            var model = serviceProvider.GetService<ProcessModel>();

            Action(serviceProvider);

            instance
                .HandleActivityCompletation(tokenId, model, null);
        }

        Activity AddDataInput(string key)
        {
            throw new NotImplementedException();
        }

    }
}