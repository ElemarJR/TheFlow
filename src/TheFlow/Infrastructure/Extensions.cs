
using System;
using System.Collections.Immutable;
using TheFlow.Elements;

// ReSharper disable once CheckNamespace
namespace TheFlow
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider sp) => 
            (T)sp.GetService(typeof(T));

        internal static bool HasDuplicatedNames(
            this ImmutableList<IProcessElement<IElement>> elements
        )
        {
            for (var i = 0; i < elements.Count - 1; i++)
            {
                if (!(elements[i] is INamedProcessElement<IElement> left)) continue;

                for (var j = i + 1; j < elements.Count; j++)
                {
                    if (!(elements[j] is INamedProcessElement<IElement> right)) continue;

                    if (left.Name.Equals(right.Name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
