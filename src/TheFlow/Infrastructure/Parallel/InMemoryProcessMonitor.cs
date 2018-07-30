using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace TheFlow.Infrastructure.Parallel
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InMemoryProcessMonitor : IProcessMonitor
    {
        private readonly ILogger<InMemoryProcessMonitor> _logger;
        
        public InMemoryProcessMonitor(ILogger<InMemoryProcessMonitor> logger)
        {
            _logger = logger;
            _lockObjects = new ConcurrentDictionary<string, object>();
        }

        private readonly ConcurrentDictionary<string, object> _lockObjects;
            
        
        public void Enter(string lockKey)
        {
            var lockObject = _lockObjects.GetOrAdd(lockKey, (s) => new object());
            Monitor.Enter(lockObject);
            _logger?.LogInformation($"Entering {lockKey}...");
        }

        public void Exit(string lockKey)
        {
            var lockObject = _lockObjects.GetOrAdd(lockKey, (s) => new object());
            Monitor.Exit(lockObject);
            _logger?.LogInformation($"Exiting {lockKey}...");
        }

        public IDisposable Lock(string lockKey)
            => new ProcessLock(this, lockKey);
    }
}