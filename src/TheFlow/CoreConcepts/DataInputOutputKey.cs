namespace TheFlow.CoreConcepts
{
    public struct DataInputOutputKey
    {
        public string ParentElementName { get; }
        public string DataInputOrOutputName { get; }

        public DataInputOutputKey(
            string parentElementName,
            string dataInputOrOutputName
        )
        {
            ParentElementName = parentElementName;
            DataInputOrOutputName = dataInputOrOutputName;
        }
    }
}