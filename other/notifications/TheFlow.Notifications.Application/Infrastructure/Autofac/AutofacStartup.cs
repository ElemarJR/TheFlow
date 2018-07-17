using System;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TheFlow.Notifications.Application.Infrastructure.Autofac
{
    public static class AutofacStartup
    {
        public static IServiceProvider ConvertToAutofac(
            this IServiceCollection services,
            params IModule[] modules
        )
        {
            var container = new ContainerBuilder();
            foreach (var module in modules)
            {
                container.RegisterModule(module);
            }
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }
    }
}
