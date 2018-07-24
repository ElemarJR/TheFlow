using System;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Activities
{
    public class LambdaActivity : Activity,
        IDataConsumer, IDataProducer
    {
        public Action<LambdaActivity, ExecutionContext> Action { get; }

        private LambdaActivity(Action<LambdaActivity, ExecutionContext> action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }
       
        public static LambdaActivity Create(
            Action action
        ) => Create(ec => action());

        public static LambdaActivity Create(
            Action<ExecutionContext> action
        ) => Create((la, ec) => action(ec));
        
        public static LambdaActivity Create(
            Action<LambdaActivity, ExecutionContext> action
        ) => new LambdaActivity(action);
        

        public override void Run(
            ExecutionContext context
            )
        {
            var model = context.ServiceProvider.GetService<ProcessModel>();

            Action(context);

            context.Instance
                .HandleActivityCompletation(context.Token.Id, model, null);
        }

        public  readonly DataOutputCollection Outputs = new DataOutputCollection();
        public readonly DataInputCollection Inputs = new DataInputCollection();

        
        public DataOutput GetDataOutputByName(string name) 
            => Outputs.FirstOrDefault(o => o.Name == name);

        public DataInput GetDataInputByName(string name) 
            => Inputs.FirstOrDefault(i => i.Name == name);
    }
}