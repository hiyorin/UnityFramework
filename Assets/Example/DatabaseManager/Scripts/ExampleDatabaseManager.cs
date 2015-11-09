using UnityEngine;
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

        public ExampleRecord (IDictionary dict) : base (dict)
        {
            var subTable = DatabaseManager.Instance.GetTable<ExampleTableSub> ();
            sub = subTable.records.FirstOrDefault (x => x.key == subKey);
            Debug.Log (sub.key);
        }
    }

    public class ExampleTable : Table<ExampleRecord>
    {
        public ExampleTable (IDictionary dict) : base (dict) {}

        protected override ExampleRecord SetupRecord (IDictionary dict)
        {
            return new ExampleRecord (dict);
        }
    }

    public class ExampleRecordSub : Record
    {
        public string key { private set; get; }
        public int value { private set; get; }

        public ExampleRecordSub (IDictionary dict) : base (dict) {}
    }

    public class ExampleTableSub : Table<ExampleRecordSub>
    {
        public ExampleTableSub (IDictionary dict) : base (dict) {}

        protected override ExampleRecordSub SetupRecord (IDictionary dict)
        {
            return new ExampleRecordSub (dict);
        }
    }

    public override bool OnSceneCreate ()
    {
        IDictionary subDict = new Dictionary<int, IDictionary> ();
        subDict.Add (0, new Dictionary<string, object> {
            {"key",     "subKeyString"},
            {"value",   1},
        });
        ExampleTableSub subTable = new ExampleTableSub (subDict);
        DatabaseManager.Instance.CreateTable (subTable);

        IDictionary mainDict = new Dictionary<int, IDictionary> ();
        mainDict.Add (0, new Dictionary<string, object> {
            {"key",     "mainKeyString"},
            {"subKey",  "subKeyString"},
            {"value",   10},
        });
        ExampleTable mainTable = new ExampleTable (mainDict);
        DatabaseManager.Instance.CreateTable (mainTable);
        return true;
    }

    public override bool OnSceneInitialize ()
    {
        var table = DatabaseManager.Instance.GetTable<ExampleTable> ();
        foreach (var record in table.records)
        {
            Debug.LogFormat ("key:{0}, subKey:{1}, value:{2}, subValue:{3}",
                record.key, record.subKey, record.value, record.sub.value);
        }
        return true;
    }
}
