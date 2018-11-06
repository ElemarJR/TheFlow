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

        public ProcessModel AddAnyEventCatcher(string name)
            => AddEventCatcher(name, CatchAnyEventCatcher.Create());

        public ProcessModel AddEventCatcher(string name, IEventCatcher catcher)
            => AddEventCatcher(NamedProcessElement<IEventCatcher>.Create(name, catcher));

        public ProcessModel AddEventCatcherFor<TEvent>()
            where TEvent : class
            => AddEventCatcherFor<TEvent>(Conventions.Naming.EventCatcher(typeof(TEvent).Name));

        public ProcessModel AddEventCatcherFor<TEvent>(string name)
            where TEvent : class
        {
            var eventCatcher = new TypedEventCatcher<TEvent>();
            var element = NamedProcessElement<IEventCatcher>.Create(name, eventCatcher);
            return AddEventCatcher(element);
        }

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


        // TODO: Create a new EventName
        public ProcessModel AddEventThrower<T>()
        {
            var thrower = (IEventThrower) Activator.CreateInstance<T>();
            var name = typeof(T).Name;
            if (name.EndsWith("EventThrower"))
            {
                name = name.Substring(0, name.Length - "EventThrower".Length);
            }

            return AddEventThrower(NamedProcessElement<IEventThrower>.Create(name, thrower));
        }




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

        public ProcessModel AddActivity<TActivity>()
            where TActivity : Activity
        {
            var activity = Activator.CreateInstance<TActivity>();
            return AddActivity(Conventions.Naming.ActivityName(typeof(TActivity).Name), activity);
        }

        public ProcessModel AddActivityWithCompensation<TActivity, TCompensation>()
            where TActivity : Activity
            where TCompensation : Activity
        {
            var nameOfActivity = Conventions.Naming.ActivityName(typeof(TActivity).Name);
            var nameOfCompensation = Conventions.Naming.ActivityName(typeof(TCompensation).Name);

            return this
                .AddActivity<TActivity>()
                .AddActivity<TCompensation>()
                .AttachAsCompensationActivity(nameOfCompensation, nameOfActivity);
        }

        public ProcessModel AddActivity<TActivity>(string name)
            where TActivity : Activity
        {
            var activity = Activator.CreateInstance<TActivity>();
            return AddActivity(name, activity);
        }

        public ProcessModel AddParallelGateway(string name)
            => AddElement(NamedProcessElement<ParallelGateway>.Create(name, new ParallelGateway()));

        public ProcessModel AddExclusiveGateway(string name)
            => AddElement(NamedProcessElement<ExclusiveGateway>.Create(name, new ExclusiveGateway()));

        public ProcessModel AddActivity(string name, Action activity)
            => AddActivity(name, LambdaActivity.Create(activity));


        public ProcessModel AddConditionalSequenceFlow(string @from, string @to, object filterValue)
            => AddElement(ProcessElement<SequenceFlow>.Create(SequenceFlow.Create(@from, @to, filterValue)));

        private ProcessModel AddElement(IProcessElement<IElement> element)
            => new ProcessModel(Guid.NewGuid().ToString(), Version + 1, Elements.Add(element), Associations);

        // TODO: Ensure that COMPENSATION has no incoming or outcoming sequence flow 
        public ProcessModel AttachAsCompensationActivity(string compensation, string to)
        {
            var that = this;

            if (that.GetElementByName("__compensation_start__") == null)
            {
                that = that
                    .AddParallelGateway("__compensation_start__")
                    .AddParallelGateway("__compensation_end__")
                    .AddEventThrower("__process_failure__")
                    .AddSequenceFlow("__compensation_end__", "__process_failure__")
                    ;
            }


            var preifname = $"__compensation_pre_if_{to}__";
            var ifname = $"__compensation_if_{to}_was_completed__";
            var endifname = $"__compensation_endif_{to}_was_completed__";

            return new ProcessModel(Guid.NewGuid().ToString(), that.Version + 1, that.Elements, that.Associations.Add(new Association(
                    compensation,
                    to,
                    AssociationType.Compensation
                )))
                .AddActivity(preifname, LambdaActivity.Create((act, ctx) =>
                    {
                        var shouldRun = ctx.Instance.WasActivityCompleted(to);
                        act.GetDataOutputByName("default").Update(ctx, ctx.Token.ExecutionPoint, shouldRun);
                    }))
                .AddExclusiveGateway(ifname)
                .AddExclusiveGateway(endifname)
                .AddSequenceFlow("__compensation_start__", preifname, ifname)
                .AddConditionalSequenceFlow(ifname, compensation, true)
                .AddSequenceFlow(compensation, endifname)
                .AddConditionalSequenceFlow(ifname, endifname, false)
                .AddSequenceFlow(endifname, "__compensation_end__");
        }

        public ProcessModel AddDataStore<T>(string name)
        {
            return AddElement(ProcessElement<EmbeddedDataStore<T>>.Create(
                name,
                element: new EmbeddedDataStore<T>(name))
            );
        }
    }
}