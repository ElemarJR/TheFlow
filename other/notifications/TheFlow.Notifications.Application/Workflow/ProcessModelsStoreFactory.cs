using TheFlow.Infrastructure.Stores;

namespace TheFlow.Notifications.Application.Workflow
{
    public class ProcessModelsStoreFactory
    {
        public IProcessModelsStore Create()
        {
            var result = new InMemoryProcessModelsStore();

            var model = MessagingProcessFactory.Create();
            result.Store(model);

            return result;
        }
    }
}
