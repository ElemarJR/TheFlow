using Raven.Client.Documents;
using TheFlow.Infrastructure.Stores;
using TheFlow.Infrastructure.Stores.InstancesStore.RavenDB;

namespace TheFlow.Notifications.Application.Workflow
{
    class ProcessInstancesStoreFactory
    {
        private readonly IDocumentStore _documentStore;

        public ProcessInstancesStoreFactory(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }
        public IProcessInstancesStore Create()
        {
            return new RavenDbInstancesStore(_documentStore);
        }
    }
}
