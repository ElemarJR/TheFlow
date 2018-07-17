using System;
using System.Collections.Generic;

namespace TheFlow.CoreConcepts
{
    public class HandleResult
    {
        public Guid ProcessModelId { get; }
        public Guid ProcessInstanceId { get; }
        public IEnumerable<Guid> AffectedTokens { get; }

        public HandleResult(
            Guid processModelId,
            Guid processInstanceId,
            IEnumerable<Guid> affectedTokens
            )
        {
            ProcessModelId = processModelId;
            ProcessInstanceId = processInstanceId;
            AffectedTokens = affectedTokens;
        }
    }
}