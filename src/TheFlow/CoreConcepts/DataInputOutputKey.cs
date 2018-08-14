using System;

namespace TheFlow.CoreConcepts
{
    public struct DataInputOutputKey : IEquatable<DataInputOutputKey>
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

        public bool Equals(DataInputOutputKey other)
        {
            return string.Equals(ParentElementName, other.ParentElementName) && string.Equals(DataInputOrOutputName, other.DataInputOrOutputName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DataInputOutputKey && Equals((DataInputOutputKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ParentElementName != null ? ParentElementName.GetHashCode() : 0) * 397) ^ (DataInputOrOutputName != null ? DataInputOrOutputName.GetHashCode() : 0);
            }
        }
    }
}