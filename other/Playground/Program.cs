using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using TheFlow;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;
using TheFlow.Infrastructure.Stores.InstancesStore.RavenDB;

public static class Program
{
    public static void Main()
    {
        var store = new DocumentStore
        {
            Database = "TheFlow",
            Urls = new[] {"http://localhost:8080"}
        };
        store.Initialize();

        var monitor = new RavenDbProcessMonitor(store);

        while (true)
        {
            Console.WriteLine("Press ENTER to lock..");
            Console.ReadLine();
            Console.WriteLine("Waiting for the lock...");

            using (var lockObject = monitor.Lock("sample/resource"))
            {
                Console.WriteLine("Locked..");
                Console.WriteLine("Press ENTER to release..");
                Console.ReadLine();
            }

            Console.WriteLine("Released...");
        }
    }
}