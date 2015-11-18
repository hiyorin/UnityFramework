using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using Framework.Data;

[CustomEditor (typeof (DatabaseManager))]
public class DatabaseManagerInspector : DatabaseManager.AbstractEditor
{
    private struct Foldout
    {
        public bool isTable;
        public bool[] isRecord;
    }

    private Foldout[] foldout;

    public override void OnInspectorGUI ()
    {
        if (foldout == null || foldout.Length != dictTable.Count)
        {
            foldout = new Foldout [dictTable.Count];
        }

        foreach (var row in dictTable.Values.Select ((table, index) => new {table, index}))
        {
            BaseTable table = row.table;
            Type tableType = table.GetType ();

            FieldInfo recordTypeInfo = tableType.GetField ("recordType");
            Type recordType = recordTypeInfo.GetValue (table) as Type;

            FieldInfo recordListInfo = tableType.GetField ("recordList", BindingFlags.NonPublic | BindingFlags.Instance);
            IList recordList = recordListInfo.GetValue (table) as IList;

            bool[] isRecord = foldout [row.index].isRecord;
            if (isRecord == null || isRecord.Length != recordList.Count)
            {
                foldout [row.index].isRecord = isRecord = new bool[recordList.Count];
            }

            EditorUtilityEx.DrawList (recordType.Name, recordList,
                ref foldout [row.index].isTable,
                (int index, object record) => {
                    
                    isRecord [index] = EditorGUILayout.Foldout (isRecord [index], "Element " + ((Record)record).GetPrimaryKey ());
                    if (isRecord [index] == true)
                        return;
                    
                    EditorGUI.indentLevel ++;

                    foreach (var propertyInfo in record.GetType ().GetProperties ())
                    {
                        string propertyName = propertyInfo.Name;
                        if (propertyInfo.PropertyType == typeof(int))
                        {
                            EditorGUILayout.IntField (propertyName, (int)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(float))
                        {
                            EditorGUILayout.FloatField (propertyName, (float)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(string))
                        {
                            EditorGUILayout.TextField (propertyName, (string)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(double))
                        {
                            EditorGUILayout.DoubleField (propertyName, (double)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(long))
                        {
                            EditorGUILayout.LongField (propertyName, (long)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(bool))
                        {
                            EditorGUILayout.Toggle (propertyName, (bool)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(Vector2))
                        {
                            EditorGUILayout.Vector2Field (propertyName, (Vector2)propertyInfo.GetValue(record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(Vector3))
                        {
                            EditorGUILayout.Vector3Field (propertyName, (Vector3)propertyInfo.GetValue(record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(Vector4))
                        {
                            EditorGUILayout.Vector4Field (propertyName, (Vector4)propertyInfo.GetValue (record, null));
                        }
                        else if (propertyInfo.PropertyType == typeof(Color))
                        {
                            EditorGUILayout.ColorField (propertyName, (Color)propertyInfo.GetValue (record, null));
                        }
                    }

                    EditorGUI.indentLevel --;
                }
            );
        }
    }
}
