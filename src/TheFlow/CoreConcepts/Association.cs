namespace TheFlow.CoreConcepts
{
    public class Association
    {
        public string FirstElement { get; }
        public string SecondElement { get; }
        public AssociationType AssociationType { get; }

        public Association(
            string firstElement,
            string secondElement,
            AssociationType associationType
            )
        {
            FirstElement = firstElement;
            SecondElement = secondElement;
            AssociationType = associationType;
        }
    }

    public enum AssociationType
    {
        Compensation
    }
}
