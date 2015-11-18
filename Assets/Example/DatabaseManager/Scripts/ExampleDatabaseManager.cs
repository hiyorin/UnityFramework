using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Scene;
using Framework.Data;
using UniLinq;

public class ExampleDatabaseManager : SceneBase
{
    public class ExampleRecord : Record
    {
        public string key { private set; get; }
        public string subKey { private set; get; }
        public int value { private set; get; }

        public ExampleRecordSub sub { private set; get; }

        public override object GetPrimaryKey ()
        {
            return key;
        }

        protected override Record RequireRecord (Type recordType, string propertyName)
        {
            Debug.Log ("RequireRecord " + recordType.ToString () + ", " + propertyName);
            if (recordType == typeof(ExampleRecordSub) && propertyName == "sub")
                return DatabaseManager.Instance.GetTable<ExampleRecordSub> ().
                    records.FirstOrDefault (x => x.key == subKey);
            return null;
        }
    }

    public class ExampleRecordSub : Record
    {
        public string key { private set; get; }
        public int value { private set; get; }

        public override object GetPrimaryKey ()
        {
            return key;
        }
    }

    public override bool OnSceneCreate ()
    {
        Debug.Log ("OnSceneCreate");
        
        IDictionary subDict = new Dictionary<int, IDictionary> ();
        subDict.Add (0, new Dictionary<string, object> {
            {"key",     "subKeyString"},
            {"value",   1},
        });
        DatabaseManager.Instance.CreateTable<ExampleRecordSub> (subDict);

        IDictionary mainDict = new Dictionary<int, IDictionary> ();
        mainDict.Add (0, new Dictionary<string, object> {
            {"key",     "mainKeyString"},
            {"subKey",  "subKeyString"},
            {"value",   10},
        });
        DatabaseManager.Instance.CreateTable<ExampleRecord> (mainDict);
        return true;
    }

    public override bool OnSceneInitialize ()
    {
        Debug.Log ("OnSceneInitialize");

        var table = DatabaseManager.Instance.GetTable<ExampleRecord> ();
        foreach (var record in table.records)
        {
            Debug.LogFormat ("key:{0}, subKey:{1}, value:{2}, subValue:{3}",
                record.key, record.subKey, record.value, record.sub.value);
        }
        return true;
    }
}
