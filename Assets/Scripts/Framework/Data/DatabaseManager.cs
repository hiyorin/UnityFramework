using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Scene;

namespace Framework.Data
{
    public class DatabaseManager : SingletonMonoBehaviour<DatabaseManager>
    {
        private readonly Dictionary<Type, BaseTable> _dictTable = new Dictionary<Type, BaseTable> ();

        protected override void OnInitialize ()
        {
            base.OnInitialize ();
            SceneManager.Instance.AddIgnoreCollection (gameObject.name);
        }

        protected override void OnFinalize ()
        {
            base.OnFinalize ();
            SceneManager.Instance.RemoveIgnoreCollection (gameObject.name);
        }

        public bool CreateTable<T> (IDictionary dict) where T:Record, new()
        {
            Type type = typeof (T);
            Table<T> table = new Table<T> (dict);
            if (_dictTable.ContainsKey (type) == true)
            {
                _dictTable [type] = table;
            }
            else {
                _dictTable.Add (type, table);
            }

            return true;
        }

        public Table<T> GetTable<T> () where T:Record, new()
        {
            Type type = typeof (T);

            BaseTable table = null;
            if (_dictTable.TryGetValue (type, out table) == false)
            {
                Debug.LogErrorFormat ("GetTable NotFound {0}", type.Name);
                return null;
            }

            return table as Table<T>;
        }

        public bool ContainsTable<T> () where T:BaseTable
        {
            Type type = typeof(T);
            return _dictTable.ContainsKey (type);
        }
    }
}