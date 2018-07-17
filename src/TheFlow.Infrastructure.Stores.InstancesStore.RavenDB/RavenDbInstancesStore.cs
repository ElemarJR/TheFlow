using System;
using System.Linq;
using Raven.Client.Documents;
using TheFlow.CoreConcepts;

namespace TheFlow.Infrastructure.Stores.InstancesStore.RavenDB
{
    public class RavenDbInstancesStore : 
        IProcessInstancesStore
    {
        private readonly IDocumentStore _documentStore;

        public RavenDbInstancesStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }
        public ProcessInstance GetById(Guid id)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<ProcessInstance>(id.ToString());
            }
        }

        public void Store(ProcessInstance instance)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(instance);
                session.SaveChanges();
            }
        }

        public int GetRunningInstancesCount()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session
                    .Query<ProcessInstance>()
                    .Count(instance => instance.IsRunning);
            }
        }

        public ProcessInstance GetProcessInstance(Guid id) 
            => GetById(id);
    }
}
