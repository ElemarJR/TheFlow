using System.Collections.Generic;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Connections;

namespace TheFlow.Elements.ConnectionsSelectors
{
    internal interface IConnectionsSelector
    {
        IEnumerable<IProcessElement<IConnectionElement>> GetRunnableConnections(
            ExecutionContext context
        );
    }
}
