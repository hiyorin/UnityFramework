using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Framework.Scene;

[CustomEditor (typeof (SceneManager))]
public class SceneManagerInspector : SceneManager.AbstractEditor
{
    private bool isFoldout_1 = false;
    private bool isFoldout_2 = false;
    private bool[] isSceneNodeFoldout;

    public override void OnInspectorGUI ()
    {
        DrawCollectionIgnoreObjectName ();
        DrawSceneNodeSet ();

        EditorGUILayout.Toggle ("isSceneLoading", instance.IsSceneLoading ());
        EditorGUILayout.Toggle ("isSubSceneChanging", instance.IsSubSceneChanging ());
    }

    private void DrawCollectionIgnoreObjectName ()
    {
        EditorUtilityEx.DrawList<string> ("CollectionIgnoreObjectName", listCollectionIgnoreObjectName, ref isFoldout_1);
    }

    private void DrawSceneNodeSet ()
    {
        if (isSceneNodeFoldout == null || isSceneNodeFoldout.Length != sceneNodeSet.nodeList.ToArray ().Length)
            isSceneNodeFoldout = new bool[sceneNodeSet.nodeList.ToArray ().Length];
        EditorUtilityEx.DrawList<SceneNode> ("SceneNodeSet", new List<SceneNode> (sceneNodeSet.nodeList), ref isFoldout_2,
            (int index, SceneNode node) => {
                isSceneNodeFoldout [index] = EditorGUILayout.Foldout (isSceneNodeFoldout [index], node.name);
                if (isSceneNodeFoldout [index] == true)
                {
                    EditorGUI.indentLevel ++;
                    EditorGUILayout.EnumPopup ("Type", node.sceneType);
                    EditorGUILayout.EnumPopup ("State", node.state);
                    EditorGUILayout.Toggle ("isActive", node.isActive);
                    EditorGUILayout.Toggle ("isVisible", node.isVisibled);
                    EditorGUI.indentLevel --;
                }
            });
    }
}
