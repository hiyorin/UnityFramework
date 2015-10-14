using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Framework.Resource;

[CustomEditor (typeof (ResourceManager))]
public class ResourceManagerInspector : ResourceManager.AbstractEditor
{
    private bool isFoldoutRequest = false;
    private bool[] isFoldoutRequestSet = null;

    private bool isFoldoutAsset = false;
    private bool[] isFoldoutAssetSet = null;

    private bool isFoldoutAssetBundle = false;
    private bool[] isFoldoutAssetBundleSet = null;

    private bool isFoldoutTexture = false;
    private Dictionary<string, bool> isFoldoutTextureSet = new Dictionary<string, bool> ();

    public override void OnInspectorGUI ()
    {
        DrawRequest ();
        DrawAsset ();
        DrawAssetBundle ();
        DrawTexture ();
    }

    private void DrawRequest ()
    {
        if (isFoldoutRequestSet == null || isFoldoutRequestSet.Length != dictRequestSet.Count)
        {
            isFoldoutRequestSet = new bool[dictRequestSet.Count];
        }
        EditorUtilityEx.DrawDictionary<ResourceRequestSet> (
            "Request", dictRequestSet, ref isFoldoutRequest,
            (int count, string key, ResourceRequestSet value) => {
                isFoldoutRequestSet [count] = EditorGUILayout.Foldout (isFoldoutRequestSet [count], key);
                if (isFoldoutRequestSet [count] == true)
                {
                    EditorGUI.indentLevel ++;
                    EditorGUILayout.Toggle ("IsComplete", value.IsComplete ());
                    bool dammy = true;
                    EditorUtilityEx.DrawList ("List", new List<ResourceRequestItem> (value.GetList ()), ref dammy,
                        (int index, ResourceRequestItem item) => {
                            EditorGUILayout.EnumPopup ("Type", item.type);
                            EditorGUILayout.TextField ("Url", item.url);
                        }
                    );
                    EditorGUI.indentLevel --;
                }
            }
        );
    }

    private void DrawAsset ()
    {
        if (isFoldoutAssetSet == null || isFoldoutAssetSet.Length != dictResourceAsset.Count)
        {
            isFoldoutAssetSet = new bool[dictResourceAsset.Count];
        }
        EditorUtilityEx.DrawDictionary<ResourceItem> (
            "Asset", dictResourceAsset, ref isFoldoutAsset,
            (int count, string key, ResourceItem resourceItem) => {
                isFoldoutAssetSet [count] = EditorGUILayout.Foldout (isFoldoutAssetSet [count], key);
                if (isFoldoutAssetSet [count] == true)
                {
                    EditorGUI.indentLevel ++;
                    EditorGUILayout.IntField ("referenctCount", resourceItem.referenceCount);
                    EditorGUILayout.ObjectField ("resource", resourceItem.resource, typeof (Object), true);
                    EditorGUI.indentLevel --;
                }
            }
        );
    }

    private void DrawAssetBundle ()
    {
        if (isFoldoutAssetBundleSet == null || isFoldoutAssetBundleSet.Length != dictResourceAssetBundle.Count)
        {
            isFoldoutAssetBundleSet = new bool[dictResourceAssetBundle.Count];
        }
        EditorUtilityEx.DrawDictionary<ResourceItem> (
            "AssetBundle", dictResourceAssetBundle, ref isFoldoutAssetBundle,
            (int count, string key, ResourceItem resourceItem) => {
                isFoldoutAssetBundleSet [count] = EditorGUILayout.Foldout (isFoldoutAssetBundleSet [count], key);
                if (isFoldoutAssetBundleSet [count] == true)
                {
                    EditorGUI.indentLevel ++;
                    EditorGUILayout.IntField ("referenctCount", resourceItem.referenceCount);
                    EditorGUILayout.ObjectField ("resource", resourceItem.resource, typeof (Object), true);
                    EditorGUI.indentLevel --;
                }
            }
        );
    }

    private void DrawTexture ()
    {
        if (isFoldoutTextureSet.Count != dictResourceTexture.Count)
        {
            isFoldoutTextureSet.Clear ();
            foreach (var key in dictResourceTexture.Keys)
                isFoldoutTextureSet.Add (key, false);
        }
        EditorUtilityEx.DrawDictionary<ResourceItem> (
            "Texture", dictResourceTexture, ref isFoldoutTexture,
            (int count, string key, ResourceItem resourceItem) => {
                isFoldoutTextureSet [key] = EditorGUILayout.Foldout (isFoldoutTextureSet [key], key);
                if (isFoldoutTextureSet [key] == true)
                {
                    EditorGUI.indentLevel ++;
                    EditorGUILayout.IntField ("referenctCount", resourceItem.referenceCount);
                    EditorGUILayout.ObjectField ("resource", resourceItem.resource, typeof (Object), true);
                    EditorGUI.indentLevel --;
                }
            }
        );
    }
}
