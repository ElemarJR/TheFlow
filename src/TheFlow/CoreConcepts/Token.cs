using System;
using System.Collections.Generic;
using System.Linq;

namespace TheFlow.CoreConcepts
{
    public class Token
    {
        public Guid ParentId { get;  }
        public Guid Id { get; }
        public string ExecutionPoint { get; set; }
        public object LastDefaultOutput { get; set; }

        private readonly List<Token> _children;
        
        public IEnumerable<Token> Children => _children;
        public bool WasReleased { get; private set; }
        public bool IsActive => !WasReleased;

        public Token(
            Guid parentId,
            Guid id,
            string executionPoint, 
            IEnumerable<Token> children = null,
            object lastDefaultOutput = null
            )
        {
            ParentId = parentId;
            Id = id;
            ExecutionPoint = executionPoint;
            LastDefaultOutput = lastDefaultOutput;
            
            _children = children?.ToList() ?? new List<Token>();
        }

        public Token AllocateChild()
        {
            if (WasReleased)
            {
                throw new InvalidOperationException("AllocationChild is not allowed after releasing.");
            }
            
                var allocateChild = new Token(
                Id,
                Guid.NewGuid(),
                ExecutionPoint
            );

            lock (this)
            {
                _children.Add(allocateChild);
            }

            return allocateChild;
        }

        public void Release()
        {
            WasReleased = true;
            //ParentId?._children.Remove(this);
        }

        public static Token Create()
            => new Token(Guid.Empty, Guid.NewGuid(), null);

        public IEnumerable<Token> GetDescendants() => 
            _children.Concat(_children.SelectMany(c => c.GetDescendants()));


        public IEnumerable<Token> GetActiveDescendants()
        {
            if (!IsActive)
            {
                return Enumerable.Empty<Token>();
            }
            
            var activeChildren = _children.Where(c => c.IsActive).ToArray();
            return activeChildren.Concat(activeChildren.SelectMany(c => c.GetActiveDescendants()));
        }

        public IEnumerable<Token> GetActionableTokens()
        {
            if (IsActive == false) return Enumerable.Empty<Token>();
            var actionableChildren = _children.SelectMany(c => c.GetActionableTokens()).ToArray();
            if (actionableChildren.Length != 0) return actionableChildren;
            return new[] {this};
        }

        public Token FindById(Guid id) => Id == id 
            ? this 
            : GetDescendants().FirstOrDefault(token => token.Id == id);
    }
}