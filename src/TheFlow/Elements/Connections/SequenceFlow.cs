namespace TheFlow.Elements.Connections
{
    public class SequenceFlow : IConnectionElement
    {
        public string From { get; }
        public string To { get; }

        private SequenceFlow(
            string from,
            string to
            )
        {
            From = @from;
            To = to;
        }

        public static SequenceFlow Create(string from, string to) 
            => new SequenceFlow(@from, to);
    }
}