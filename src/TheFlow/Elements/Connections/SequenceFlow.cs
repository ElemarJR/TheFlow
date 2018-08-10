using Microsoft.VisualBasic.CompilerServices;

namespace TheFlow.Elements.Connections
{
    public class SequenceFlow : IConnectionElement
    {
        public string From { get; }
        public string To { get; }
        
        public object FilterValue { get; }
        public bool HasFilterValue { get; }

        private SequenceFlow(
            string from,
            string to, object filterValue, bool hasFilterValue)
        {
            From = @from;
            To = to;
            FilterValue = filterValue;
            HasFilterValue = hasFilterValue;
        }

        public static SequenceFlow Create(string from, string to) 
            => new SequenceFlow(@from, to, null, false);
        
        public static SequenceFlow Create(string from, string to, object filterValue)
            => new SequenceFlow(from, to, filterValue, true);
    }
}