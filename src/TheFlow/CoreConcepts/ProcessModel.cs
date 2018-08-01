using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Schema;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Connections;
using TheFlow.Elements.Data;
using TheFlow.Elements.Events;

namespace TheFlow.CoreConcepts
{
    public partial class ProcessModel : IProcessModelProvider
    {
        public string Id { get; }
        public int Version { get; }
        
        public ImmutableList<IProcessElement<IElement>> Elements { get; }

        #region Constructor and Empty Object
        public ProcessModel(
            string id,
            int version,
            ImmutableList<IProcessElement<IElement>> elements)
        {
            if (elements.HasDuplicatedNames())
            {
                throw new ArgumentException("There are elements using a same name.");
            }
            Id = id;
            Version = version;
            Elements = elements;
        }
        
//        
//        public static readonly ProcessModel Empty = new ProcessModel(
//            ImmutableList<IProcessElement<IElement>>.Empty
//        );  
        #endregion
        
                

        public IEnumerable<IProcessElement<IConnectionElement>> GetIncomingConnections(
            string elementName
        )
            => Elements
                .OfType<IProcessElement<IConnectionElement>>()
                .Where(c => c.Element.To == elementName);
        
        public IEnumerable<INamedProcessElement<IEventCatcher>> GetStartEventCatchers() 
            => Elements
                .OfType<INamedProcessElement<IEventCatcher>>()
                .Where(catcher => !GetIncomingConnections(catcher.Name).Any());

        public IEnumerable<INamedProcessElement<IEventThrower>> GetEndEventThrowers()
            => Elements
                .OfType<INamedProcessElement<IEventThrower>>()
                .Where(thrower => !GetOutcomingConnections(thrower.Name).Any());

        public IEnumerable<IProcessElement<IConnectionElement>> GetOutcomingConnections(
            string elementName
        ) 
        => Elements
            .OfType<IProcessElement<IConnectionElement>>()
            .Where(c => c.Element.From == elementName);

        public INamedProcessElement<IElement> GetElementByName(string name) => Elements
            .OfType<INamedProcessElement<IElement>>()
            .FirstOrDefault(e => e.Name == name);

        public bool TryToGetEndEventThrower(
            string name,
            out INamedProcessElement<IEventThrower> output
            )
        {
            output = GetEndEventThrowers()
                .FirstOrDefault(thrower => thrower.Name == name);

            return output != null;
        }
        
        public bool TryToGet(
            string name,
            out INamedProcessElement<IElement> output
        )
        {
            output = Elements
                .OfType<INamedProcessElement<IElement>>()
                .FirstOrDefault(thrower => thrower.Name == name);

            return output != null;
        }

        public bool IsEndEventThrower(string name) => 
            (TryToGetEndEventThrower(name, out var _));

        public static ProcessModel Create() =>
            Create(Guid.NewGuid());

        public static ProcessModel Create(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("ProcessModel's Id cannot be Empty", nameof(guid));
            }
            
            return new ProcessModel(
                guid.ToString(),
                0,
                ImmutableList.Create<IProcessElement<IElement>>()
            );
        }

        public bool CanStartWith(ExecutionContext context, object eventData) => 
            GetStartEventCatchers().Any(catcher => catcher.Element.CanHandle(context, eventData));

        public ProcessModel GetProcessModel(Guid id) => 
            id == Guid.Parse(Id) ? this : null;

        public static ProcessModel CreateWithSingleActivity(Activity activity) => Create()
            .AddEventCatcher("start", CatchAnyEventCatcher.Create())        
            .AddActivity("activity", activity)    
            .AddEventThrower("end", SilentEventThrower.Instance)
            .AddSequenceFlow("start", "activity", "end");
        
    }
}