using System;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Activities
{
    public class LambdaActivity : Activity
    {
        public Action<LambdaActivity, ExecutionContext> Action { get; }

        private LambdaActivity(Action<LambdaActivity, ExecutionContext> action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }
       
        public static LambdaActivity Create(
            Action action
        )
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            return Create(ec => action());
        }

        public static LambdaActivity Create(
            Action<ExecutionContext> action
        )
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            return Create((la, ec) => action(ec));
        }

        public static LambdaActivity Create(
            Action<LambdaActivity, ExecutionContext> action
        )
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            return new LambdaActivity(action);
        }


        public override void Run(
            ExecutionContext context
            )
        {
            var model = context.Model;

            Action(this, context);

            context.Instance
                .HandleActivityCompletion(context, null);
        }

    }
}