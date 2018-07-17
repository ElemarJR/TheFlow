
using System;

// ReSharper disable once CheckNamespace
namespace TheFlow
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider sp) => 
            (T)sp.GetService(typeof(T));
    }
}
