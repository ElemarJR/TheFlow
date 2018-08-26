using System;
using System.Collections.Generic;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Connections;

namespace TheFlow.Elements.ConnectionsSelectors
{
    internal class DefaultConnectionsSelector : IConnectionsSelector
    {
        private DefaultConnectionsSelector()
        {
        }

        private static readonly Lazy<DefaultConnectionsSelector> LazyInstance =
            new Lazy<DefaultConnectionsSelector>(() => new DefaultConnectionsSelector());

        public static IConnectionsSelector Instance => LazyInstance.Value;

        public IEnumerable<IProcessElement<IConnectionElement>> GetRunnableConnections(
            ExecutionContext context
            )
        {
            var connections = context.Model
                .GetOutcomingConnections(context.Token.ExecutionPoint)
                .ToArray();

            return connections
                .Where(connection =>
                {
                    if (!connection.Element.HasFilterValue)
                    {
                        return true;
                    }

                    if (connection.Element.FilterValue == null)
                    {
                        return context.Token.LastDefaultOutput == null;
                    }

                    return connection.Element.FilterValue.Equals(
                        context.Token.LastDefaultOutput
                    );
                });
        }

    }
}