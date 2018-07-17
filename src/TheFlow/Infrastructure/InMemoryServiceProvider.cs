using System;
using Microsoft.Extensions.DependencyInjection;

namespace TheFlow.Infrastructure
{
    public class CumulativeServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _primarySource;
        private readonly IServiceProvider _secondarySource;

        public CumulativeServiceProvider(
            IServiceProvider primarySource,
            IServiceProvider secondarySource
            
            )
        {
            _primarySource = primarySource;
            _secondarySource = secondarySource;
        }

        public object GetService(Type serviceType) => 
            _primarySource.GetService(serviceType) 
            ?? _secondarySource.GetService(serviceType);
    }
}
