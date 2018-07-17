using System;

namespace TheFlow.Elements.Data
{
    public class SimpleDataElementFactory<T> : IDataElementFactory
    {
        private SimpleDataElementFactory() {}
        
        public T CreateInstance() => Activator
            .CreateInstance<T>();

        object IDataElementFactory.CreateInstance() => Activator
            .CreateInstance<T>();
        
        public static SimpleDataElementFactory<T> Instance =
            new SimpleDataElementFactory<T>();

        public void AddDataOutput(string key)
        {
            throw new NotImplementedException();
        }

        public void AddDataInput(string key)
        {
            throw new NotImplementedException();
        }
    }
}