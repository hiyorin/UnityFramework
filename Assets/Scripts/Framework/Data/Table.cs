using System.Collections;
using System.Collections.Generic;

namespace Framework.Data
{
    public abstract class ITable {}

    public abstract class Table<T> : ITable where T:Record
    {
        protected readonly IList<T> recordList = new List<T> ();

        public Table (IDictionary dict)
        {
            foreach (var row in dict.Values)
            {
                T record = SetupRecord (row as IDictionary);
                recordList.Add (record);
            }
        }

        protected abstract T SetupRecord (IDictionary dict);

        public IEnumerable<T> records {
            get { return recordList; }
        }
    }
}