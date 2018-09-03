namespace TheFlow.CoreConcepts.Names
{
    public struct DataObjectName
    {
        private readonly string _name;

        private DataObjectName(string name)
        {
            _name = name;
        }

        public static implicit operator DataObjectName(string input)
        {
            var dataObjectName = input;
            if ((dataObjectName.StartsWith("On") || dataObjectName.StartsWith("on")) &&
                char.IsUpper(dataObjectName[2]))

            {
                dataObjectName = dataObjectName.Substring(2);
            }

            return new DataObjectName(dataObjectName);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}