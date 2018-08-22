using System;

namespace TheFlow.Infrastructure.Parallel
{
    // TODO: Validate the parameters
    public class ProcessLock : IDisposable
    {
        private readonly IProcessMonitor _monitor;
        private readonly string _lockKey;
        private readonly long _lockIndex;

        public ProcessLock(
            IProcessMonitor monitor,
            string lockKey
            )
        {
            _monitor = monitor;
            _lockKey = lockKey;
            
            _lockIndex = _monitor.Enter(lockKey);
        }

        public void Dispose()
        {
            _monitor.Exit(_lockKey, _lockIndex);
        }
    }
}