using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Framework;

public class EditorUtilityEx : Editor
{
    public static void DrawList (string label, IList list, ref bool isFoldout, Action<int, object> callback=null)
    {
        isFoldout = EditorGUILayout.Foldout (isFoldout, label);
        if (isFoldout == true)
        {
            EditorGUI.indentLevel ++;
            EditorGUILayout.IntField ("Size", list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                if (callback != null)
                    callback.Invoke (i, list [i]);
            }
            EditorGUI.indentLevel --;
        }
    }

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

    public static void DrawEidtList<T> (string label, List<T> list, ref bool isFoldout, Action<int, T> callback=null)
    {
        isFoldout = EditorGUILayout.Foldout (isFoldout, label);
        if (isFoldout == true)
        {
            EditorGUI.indentLevel ++;
            int count = EditorGUILayout.IntField ("Size", list.Count);
            while (count != list.Count)
            {
                if (count < list.Count)
                    list.RemoveAt (list.Count - 1);
                else
                    list.Add (default (T));
            }
            EditorGUILayout.IntField ("Size", list.Count);
            foreach (var row in list.Select ((value, index) => new {value, index}))
            {
                T valueBuffer = row.value;
                if (DrawEditValue<T> ("Element " + row.index, ref valueBuffer) == true)
                {
                    list [row.index] = valueBuffer;
                }
                else
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

    private static bool DrawEditValue<T> (string label, ref T value)
    {
        if (typeof(T) == typeof(string))
            ParseUtility.TryParse<string, T> (
                EditorGUILayout.TextField (label, value.ToString ()),
                ref value);
        else if (typeof(T) == typeof(int))
            ParseUtility.TryParse<int, T> (
                EditorGUILayout.IntField (label, int.Parse (value.ToString ())),
                ref value);
        else if (typeof(T) == typeof(float))
            ParseUtility.TryParse<float, T> (
                EditorGUILayout.FloatField (label, float.Parse (value.ToString ())),
                ref value);
        else if (typeof(T) == typeof(double))
            ParseUtility.TryParse<double, T> (
                EditorGUILayout.DoubleField (label, double.Parse (value.ToString ())),
                ref value);
        else if (typeof(T) == typeof(long))
            ParseUtility.TryParse<long, T> (
                EditorGUILayout.LongField (label, long.Parse (value.ToString ())),
                ref value);
        else if (typeof(T) == typeof(UnityEngine.Object))
            ParseUtility.TryParse<UnityEngine.Object, T> (
                EditorGUILayout.ObjectField (label, value as UnityEngine.Object, typeof(T), true),
                ref value);
        else
            return false;
        return true;
    }
}
