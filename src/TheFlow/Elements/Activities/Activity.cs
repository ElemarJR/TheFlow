using System;

namespace TheFlow.Elements.Activities
{
    public abstract class Activity : IElement
    {
        public abstract void Run(
            IServiceProvider serviceProvicer, 
            Guid instanceId, 
            Guid tokenId
            );
    }
}