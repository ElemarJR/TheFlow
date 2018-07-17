using Raven.Client.Documents.Indexes;

namespace TheFlow.Notifications.Application.QuerySide.Indexes
{
    public class NotificationsUnprocessed
        : AbstractIndexCreationTask
    {
        public override string IndexName => "Notifications/Unprocessed";

        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition()
            {
                Maps = {
                    @"from instance in docs.ProcessInstances
                    select new {
                        IsRunning = instance.IsRunning,
                        To = instance.History[0].Payload.To
                    }"
                    }
            };
        }
    }
}
