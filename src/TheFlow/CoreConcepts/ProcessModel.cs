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
    public class ProcessModel : IProcessModelProvider
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
        
        public ProcessModel AddNullElement(string name, NullElement n)
            => new ProcessModel(Id, Version + 1, Elements.Add(NamedProcessElement<NullElement>.Create(name, n)));

        public ProcessModel AddEventCatcher(string name, IEventCatcher catcher)
            => AddEventCatcher(NamedProcessElement<IEventCatcher>.Create(name, catcher));
        
        public ProcessModel AddEventCatcher(NamedProcessElement<IEventCatcher> catcher) 
            => new ProcessModel(Id, Version + 1, Elements.Add(catcher));

        public ProcessModel AddEventThrower(string name, IEventThrower thrower)
            => AddEventThrower(NamedProcessElement<IEventThrower>.Create(name, thrower));
        
        public ProcessModel AddEventThrower(NamedProcessElement<IEventThrower> thrower) 
            => new ProcessModel(Id, Version + 1, Elements.Add(thrower));

        // validate null
        public ProcessModel AddDataAssociation(
            string name,
            DataAssociation association
        )
        {
            return new ProcessModel(Id,
                Version + 1,
                Elements.Add(NamedProcessElement<DataAssociation>.Create(name, association))
            );
        }
        public ProcessModel AddSequenceFlow(
            string from,
            string to,
            params string []  path)
        {
            var result = AddSequenceFlow(SequenceFlow.Create(from, to));
            from = to;
            for (var i = 0; i < path.Length; i++)
            {
                result = result.AddSequenceFlow(SequenceFlow.Create(from, path[i]));
                from = path[i];
            }
            return result;
        }
            

        public ProcessModel AddSequenceFlow(SequenceFlow sequenceFlow)
            => AddSequenceFlow(ProcessElement<SequenceFlow>.Create(sequenceFlow));
        
        public ProcessModel AddSequenceFlow(ProcessElement<SequenceFlow> sequenceFlow)
            => new ProcessModel(Id, Version + 1, Elements.Add(sequenceFlow));

        public ProcessModel AddActivity(string name, Activity activity)
            => AddActivity(ProcessElement<Activity>.Create(name, activity));
        
        public ProcessModel AddActivity(NamedProcessElement<Activity> activity) 
            => new ProcessModel(Id, Version + 1, Elements.Add(activity));

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

        // TODO: What should we do with a empty guid.
        public static ProcessModel Create(Guid guid) => new ProcessModel(
            guid.ToString(),
            0,
            ImmutableList.Create<IProcessElement<IElement>>()
        );

        // TODO: what pass to the service provider of a starting event?
        public bool CanStartWith(object eventData) => 
            GetStartEventCatchers().Any(catcher => catcher.Element.CanHandle(null, eventData));

        public ProcessModel GetProcessModel(Guid id) => 
            id == Guid.Parse(Id) ? this : null;

        public static ProcessModel CreateWithSingleActivity(LambdaActivity activity) => Create()
            .AddEventCatcher("start", CatchAnyEventCatcher.Create())
            .AddActivity("activity", activity)
            .AddEventThrower("end", SilentEventThrower.Instance)
            .AddSequenceFlow("start", "activity", "end");

    }
}