﻿using System;
using System.Collections.Generic;
using Framework.Scene;

namespace Framework.Data
{
    public class DatabaseManager : SingletonMonoBehaviour<DatabaseManager>
    {
        private readonly Dictionary<Type, ITable> _dictTable = new Dictionary<Type, ITable> ();

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

        public bool CreateTable (ITable table)
        {
            if (table == null)
            {
                return false;
            }

            Type type = table.GetType ();
            if (_dictTable.ContainsKey (type) == true)
            {
                _dictTable [type] = table;
            }
            else {
                _dictTable.Add (type, table);
            }

            return true;
        }

        public T GetTable<T> () where T:ITable
        {
            Type type = typeof (T);

            ITable table = null;
            if (_dictTable.TryGetValue (type, out table) == false)
            {
                return default (T);
            }

            return table as T;
        }

        public bool ContainsTable<T> () where T:ITable
        {
            Type type = typeof(T);
            return _dictTable.ContainsKey (type);
        }
    }
}