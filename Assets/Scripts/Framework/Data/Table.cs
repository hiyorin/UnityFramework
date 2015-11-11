using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Data
{
    public abstract class BaseTable {}

    public class Table<T> : BaseTable where T:Record, new()
    {
        protected readonly IList<T> recordList = new List<T> ();

        public IEnumerable<T> records {
            get { return recordList; }
        }

        public Table (IDictionary dict)
        {
            foreach (var row in dict.Values)
            {
                T record = new T ();
                Record.SetupRecord (record, row as IDictionary);
                recordList.Add (record);
            }
        }
    }
}