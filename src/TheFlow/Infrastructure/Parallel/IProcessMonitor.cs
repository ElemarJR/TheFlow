using System;

namespace TheFlow.Infrastructure.Parallel
{
    public interface IProcessMonitor
    {
        long Enter(string lockKey);
        void Exit(string lockKey, long lockIndex);
        IDisposable Lock(string lockKey);
    }
}