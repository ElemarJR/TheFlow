using System;
using Raven.Client.Documents;
using TheFlow.Infrastructure.Stores.InstancesStore.RavenDB;

namespace Playground
{
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

                using (monitor.Lock("sample/resource"))
                {
                    Console.WriteLine("Locked..");
                    Console.WriteLine("Press ENTER to release..");
                    Console.ReadLine();
                }

                Console.WriteLine("Released...");
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}