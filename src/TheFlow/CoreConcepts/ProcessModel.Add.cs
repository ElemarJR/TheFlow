using System;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Connections;
using TheFlow.Elements.Data;
using TheFlow.Elements.Events;
using TheFlow.Elements.Gateways;

namespace TheFlow.CoreConcepts
{
    public partial class ProcessModel
    {
        public ProcessModel AddNullElement(string name, NullElement n)
            => new ProcessModel(Id, Version + 1, Elements.Add(NamedProcessElement<NullElement>.Create(name, n)));

        public ProcessModel AddEventCatcher(string name) 
            => AddEventCatcher(name, CatchAnyEventCatcher.Create());

        public ProcessModel AddEventCatcher(string name, IEventCatcher catcher)
            => AddEventCatcher(NamedProcessElement<IEventCatcher>.Create(name, catcher));
        
        public ProcessModel AddEventCatcher(NamedProcessElement<IEventCatcher> catcher) 
            => new ProcessModel(Id, Version + 1, Elements.Add(catcher));

        public ProcessModel AddEventThrower(string name)
            => AddEventThrower(name, SilentEventThrower.Instance);
        
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
        
        public ProcessModel AddParallelGateway(string name)
            => new ProcessModel(Id, Version + 1, Elements.Add(NamedProcessElement<ParallelGateway>.Create(name, new ParallelGateway())));

        public ProcessModel AddActivity(string name, Action activity)
            => AddActivity(name, LambdaActivity.Create(activity));
    }
}