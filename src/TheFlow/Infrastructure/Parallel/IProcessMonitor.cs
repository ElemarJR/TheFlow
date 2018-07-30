using System;

namespace TheFlow.Infrastructure.Parallel
{
    public interface IProcessMonitor
    {
        void Enter(string lockKey);
        void Exit(string lockKey);
        IDisposable Lock(string lockKey);
    }
}