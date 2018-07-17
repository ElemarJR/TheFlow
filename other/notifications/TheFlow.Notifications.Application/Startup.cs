using System;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using TheFlow.Notifications.Application.Infrastructure.Autofac;
using TheFlow.Notifications.Application.Infrastructure.Idempotency;
using TheFlow.Notifications.Application.Infrastructure.Mediatr.Autofac;
using TheFlow.Notifications.Application.QuerySide.Indexes;
using TheFlow.Notifications.Application.Workflow;

namespace TheFlow.Notifications.Application
{
    public static class Startup
    {
        public static IServiceProvider AddApplication(
            this IServiceCollection services
        )
        {
            services.AddSingleton<IDocumentStore>((sp) =>
            {
                var store = new DocumentStore
                {
                    Urls = new[] {"http://localhost:8080"},
                    Database = "Notifications"
                }.Initialize();

                new NotificationsUnprocessed().Execute(store);

                return store;
            });

            services.AddSingleton<ProcessModelsStoreFactory>();
            services.AddSingleton((sp) =>
            {
                var factory = sp.GetService<ProcessModelsStoreFactory>();
                return factory.Create();
            });
            
            services.AddSingleton<ProcessInstancesStoreFactory>();
            services.AddSingleton((sp) =>
            {
                var factory = sp.GetService<ProcessInstancesStoreFactory>();
                return factory.Create();
            });
            
            services.AddSingleton<ProcessManager>();

            services.AddSingleton<IRequestManager>(new InMemoryRequestManager());

            return services.ConvertToAutofac(
                MediatrModule.Create(typeof(Startup).Assembly)
            );
        }
    }
}
