using System;

namespace TheFlow.Conventions
{
    public class ProcessModelNamingConventions
    {
        public Func<string, string> ActivityName { get; set; } = input =>
            input.EndsWith("Activity")
                ? input.Substring(0, input.Length - "Activity".Length)
                : input;

        public Func<string, string> DataObjectName { get; set; } = input =>
            (input.StartsWith("On") || input.StartsWith("on")) && char.IsUpper(input[2])
                ? input.Substring(2)
                : input;

        public Func<string, string> EventCatcher { get; set; } = input => 
            (input.StartsWith("On") || input.StartsWith("on")) && char.IsUpper(input[2])
                ? input
                : $"On{input}";

        public Func<string, string> EventThrower { get; set; } = input =>
            input.EndsWith("EventThrower")
                ? input.Substring(0, input.Length - "EventThrower".Length)
                : input;
    }
}