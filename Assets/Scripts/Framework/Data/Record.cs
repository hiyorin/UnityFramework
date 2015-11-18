using UnityEngine;
using System;
using System.Collections;
using System.ComponentModel;

namespace Framework.Data
{
    public abstract class Record
    {
        public abstract object GetPrimaryKey ();

        protected virtual Record RequireRecord (Type recordType, string propertyName)
        {
            return null;
        }

        public static void SetupRecord (Record record, IDictionary dict)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties (record))
            {
                Type type = property.PropertyType;

                if (type.BaseType == typeof(Record))
                {
                    Record requireRecord = record.RequireRecord (type, property.Name);
                    property.SetValue (record, requireRecord);
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
