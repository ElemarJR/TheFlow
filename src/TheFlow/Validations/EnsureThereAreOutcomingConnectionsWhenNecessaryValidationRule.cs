using System.Collections.Generic;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;

namespace TheFlow.Validations
{
    public class EnsureThereAreOutcomingConnectionsWhenNecessaryValidationRule
    {
        public IEnumerable<ValidationFail> Validate(ProcessModel model) => model.Elements
            .OfType<INamedProcessElement<IElement>>()
            .Where(s => (
                s.Element is Activity || 
                s.Element is IEventCatcher  
            ))
            .Where(s => !model.GetOutcomingConnections(s.Name).Any())
            .Select(s => new ValidationFail($"'{s.Name}' has no outgoing connections."));
    }
}