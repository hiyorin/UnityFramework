using UnityEngine;
using System;
using System.Collections;
using System.ComponentModel;

namespace Framework.Data
{
    public abstract class Record
    {
        public Record (IDictionary dict)
        {
            SetupRecord (this, dict);
        }

        protected static void SetupRecord (Record record, IDictionary dict)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties (record))
            {
                Type type = property.PropertyType;

                if (type.BaseType == typeof(Record))
                {
                    SetupRecord (property.GetValue (record) as Record, dict [property.Name] as IDictionary);
                }
                else
                {
                    try
                    {
                        string stringValue = Convert.ToString (dict [property.Name]);
                        object value = Convert.ChangeType (stringValue, type);
                        property.SetValue (record, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException (e);
                    }
                }
            }
        }
    }
}
