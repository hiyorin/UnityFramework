using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public class EditorUtilityEx : Editor
{
    public static void DrawList<T> (string label, List<T> list, ref bool isFoldout, Action<int, T> callback=null)
    {
        isFoldout = EditorGUILayout.Foldout (isFoldout, label);
        if (isFoldout == true)
        {
            EditorGUI.indentLevel ++;
            EditorGUILayout.IntField ("Size", list.Count);
            foreach (var row in list.Select ((value, index) => new {value, index}))
            {
                if (DrawValue<T> ("Element " + row.index, row.value) == false)
                {
                    if (callback != null)
                        callback.Invoke (row.index, row.value);
                }
            }
            EditorGUI.indentLevel --;
        }
    }

    public static void DrawDictionary<T> (string label, Dictionary<string, T> dict, ref bool isFoldout, Action<int, string, T> callback=null)
    {
        isFoldout = EditorGUILayout.Foldout (isFoldout, label);
        if (isFoldout == true)
        {
            EditorGUI.indentLevel ++;
            EditorGUILayout.IntField ("Size", dict.Count);
            foreach (var row in dict.Keys.Select ((key, index) => new {key, index}))
            {
                if (DrawValue<T> (row.key, dict [row.key]) == false)
                {
                    if (callback != null)
                        callback.Invoke (row.index, row.key, dict [row.key]);
                }
            }
            EditorGUI.indentLevel --;
        }
    }

    private static bool DrawValue<T> (string label, T value)
    {
        if (typeof(T) == typeof(string))
            EditorGUILayout.TextField (label, value.ToString ());
        else if (typeof(T) == typeof(int))
            EditorGUILayout.IntField (label, int.Parse (value.ToString ()));
        else if (typeof(T) == typeof(float))
            EditorGUILayout.FloatField (label, float.Parse (value.ToString ()));
        else if (typeof(T) == typeof(double))
            EditorGUILayout.DoubleField (label, double.Parse (value.ToString ()));
        else if (typeof(T) == typeof(long))
            EditorGUILayout.LongField (label, long.Parse (value.ToString ()));
        else if (typeof(T) == typeof(UnityEngine.Object))
            EditorGUILayout.ObjectField (label, value as UnityEngine.Object, typeof(T), true);
        else
            return false;
        return true;
    }
}
