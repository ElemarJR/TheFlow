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
            => AddElement(NamedProcessElement<NullElement>.Create(name, n));

        public ProcessModel AddEventCatcher(string name)
            => AddEventCatcher(name, CatchAnyEventCatcher.Create());

        public ProcessModel AddEventCatcher(string name, IEventCatcher catcher)
            => AddEventCatcher(NamedProcessElement<IEventCatcher>.Create(name, catcher));

        public ProcessModel AddEventCatcher(NamedProcessElement<IEventCatcher> catcher)
            => AddElement(catcher);

        public ProcessModel AddBoundaryEventCatcher(
            NamedProcessElement<IEventCatcher> catcher,
            string activityName
        ) => AddElement(catcher);

        public ProcessModel AddEventThrower(string name)
            => AddEventThrower(name, SilentEventThrower.Instance);

        public ProcessModel AddEventThrower(string name, IEventThrower thrower)
            => AddEventThrower(NamedProcessElement<IEventThrower>.Create(name, thrower));

        public ProcessModel AddEventThrower(NamedProcessElement<IEventThrower> thrower)
            => AddElement(thrower);

        // validate null
        public ProcessModel AddDataAssociation(
            string name,
            DataAssociation association
        ) => AddElement(
            NamedProcessElement<DataAssociation>.Create(name, association)
        );

        public ProcessModel AddSequenceFlow(
            string from,
            string to,
            params string[] path)
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
            => AddElement(sequenceFlow);

        public ProcessModel AddActivity(string name, Activity activity)
            => AddElement(ProcessElement<Activity>.Create(name, activity));

        public ProcessModel AddActivity(NamedProcessElement<Activity> activity)
            => AddElement(activity);

        public ProcessModel AddParallelGateway(string name)
            => AddElement(NamedProcessElement<ParallelGateway>.Create(name, new ParallelGateway()));

        public ProcessModel AddExclusiveGateway(string name)
            => AddElement(NamedProcessElement<ExclusiveGateway>.Create(name, new ExclusiveGateway()));

        public ProcessModel AddActivity(string name, Action activity)
            => AddActivity(name, LambdaActivity.Create(activity));


        public ProcessModel AddConditionalSequenceFlow(string @from, string @to, object filterValue)
            => AddElement(ProcessElement<SequenceFlow>.Create(SequenceFlow.Create(@from, @to, filterValue)));

        private ProcessModel AddElement(IProcessElement<IElement> element)
            => new ProcessModel(Id, Version + 1, Elements.Add(element), Associations);

        public ProcessModel AttachAsCompensationActivity(string compensationActivityName, string regularActivityName)
            => new ProcessModel(Id, Version + 1, Elements, Associations.Add(new Association(
                compensationActivityName,
                regularActivityName,
                AssociationType.Compensation
            )));
    }
}