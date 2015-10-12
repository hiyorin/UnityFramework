using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Framework.Scene;
using UniLinq;

[CustomEditor (typeof (SceneProperty))]
public class ScenePropertyInspector : Editor
{
    private static Dictionary<string, GameObject> LoadSubSceneDictionary = new Dictionary<string, GameObject> ();
    private static HideFlags HideFlag = (HideFlags.NotEditable | HideFlags.DontSaveInEditor);
    private static bool IsHierarchyWindowChanged = false;

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad ()
    {
        EditorApplication.update += OnUpdate;
        EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
        EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
    }

    private static void OnUpdate ()
    {
        // Hierarchyが変更されたらサブシーンのHideFlagsを更新する
        if (IsHierarchyWindowChanged == true)
        {
            foreach (var go in LoadSubSceneDictionary.Values)
            {
                foreach (var child in go.GetComponentsInChildren<Transform> ())
                    child.gameObject.hideFlags |= HideFlag;
                go.hideFlags |= HideFlag;
            }
            IsHierarchyWindowChanged = false;
        }

        // コンパイルが走り始めたらサブシーンを全て破棄する
        if (EditorApplication.isCompiling == false)
            return;
        if (LoadSubSceneDictionary.Count <= 0)
            return;
        foreach (var go in LoadSubSceneDictionary.Values)
        {
            foreach (var child in go.GetComponentsInChildren<Transform> ())
                child.gameObject.hideFlags |= HideFlag;
            go.hideFlags |= HideFlag;
            DestroyImmediate (go);
        }
        LoadSubSceneDictionary.Clear ();
    }

    private static void OnPlayModeStateChanged ()
    {
        if (Application.isPlaying == true)
            return;
        
        if (EditorApplication.isPlayingOrWillChangePlaymode == true)
        {
            // プレイ開始時にサブシーンをHierarchyから消す
            foreach (var go in LoadSubSceneDictionary.Values) {
                go.SetActive (false);
                go.hideFlags |= HideFlags.HideInHierarchy;
            }
        }
        else
        {
            // プレイ終了時にサブシーンをHierarchyに表示する
            foreach (var go in LoadSubSceneDictionary.Values)
            {
                go.SetActive (true);
                go.hideFlags &= ~(HideFlags.HideInHierarchy);
            }
        }
    }

    private static void OnHierarchyWindowChanged ()
    {
        // Hierarchyの変更をフックしてHierarchyに変更を加えると即時反映されないのでUpdateで実行する
        IsHierarchyWindowChanged = true;
    }

	public override void OnInspectorGUI()
	{
        SceneProperty property = target as SceneProperty;

        AlwaysDestroy (property);
        EditorGUILayout.Space ();
        AddSubScene (property);

        if (Application.isPlaying == false && EditorApplication.isCompiling == false)
            AdditiveScene (property);
	}

    /// <summary>
    /// 遷移後の挙動
    /// </summary>
    /// <param name="property">Property.</param>
    private void AlwaysDestroy (SceneProperty property)
    {
        property.isAlwaysDestroy = EditorGUILayout.Toggle ("isAlwaysDestroy", property.isAlwaysDestroy);
    }

    /// <summary>
    /// 追加するサブシーン
    /// </summary>
    /// <param name="property">Property.</param>
    private void AddSubScene (SceneProperty property)
    {
        int currentSceneIndex = EditorApplication.currentScene.LastIndexOf ("/");
        string currentSceneName = EditorApplication.currentScene.Substring (currentSceneIndex + 1).Replace (".unity", "");

        EditorGUILayout.LabelField ("Add Sub Scene");

        EditorGUILayout.BeginVertical ("box");
        EditorGUILayout.LabelField ("SceneName", "isAdd, isVisible");
        EditorGUILayout.BeginVertical ("box");
        foreach (var scene in EditorBuildSettings.scenes)
        {
            int lastIndex = scene.path.LastIndexOf ("/");
            string sceneName = scene.path.Substring (lastIndex + 1).Replace (".unity", "");
            if (sceneName.Equals (currentSceneName) == true)
                continue;
            if (SceneManagerSettings.ListResidentSceneName.Contains (sceneName) == true)
                continue;
            
            EditorGUILayout.BeginHorizontal ();
            bool isContains = property.listAddSubScene.Any (x => x.sceneName == sceneName);

            if (EditorGUILayout.Toggle (sceneName, isContains) == true)
            {
                if (isContains == false)
                    property.listAddSubScene.Add (new SubSceneProperty (sceneName, true, scene.path));
            }
            else
            {
                SubSceneProperty subSceneProperty = property.listAddSubScene.FirstOrDefault (x => x.sceneName == sceneName);
                if (subSceneProperty != null)
                {
                    property.listAddSubScene.Remove (subSceneProperty);
                    RemoveAdditiveScene (property, sceneName);
                }
            }

            bool isVisible = false;
            if (isContains == true)
            {
                SubSceneProperty subSceneProperty = property.listAddSubScene.FirstOrDefault (x => x.sceneName == sceneName);
                if (subSceneProperty != null)
                    isVisible = subSceneProperty.isVisible;
            }

            isVisible = EditorGUILayout.Toggle (isVisible);
            if (isContains == true)
            {
                SubSceneProperty subSceneProperty = property.listAddSubScene.FirstOrDefault (x => x.sceneName == sceneName);
                if (subSceneProperty != null)
                    subSceneProperty.isVisible = isVisible;
            }
            EditorGUILayout.EndHorizontal ();
        }
        EditorGUILayout.EndVertical ();
        EditorGUILayout.EndVertical ();
    }

    private void AdditiveScene (SceneProperty property)
    {
        foreach (var subScene in property.listAddSubScene)
        {
            if (subScene.isVisible)
            {
                if (LoadSubSceneDictionary.Keys.Contains (subScene.sceneName) == false)
                {
                    GameObject rootObject = LoadWithCollection (subScene.sceneName, subScene.sceneFullName);
                    LoadSubSceneDictionary.Add (subScene.sceneName, rootObject);
                }
            }
            else
            {
                RemoveAdditiveScene (property, subScene.sceneName);
            }
        }
    }

    private void RemoveAdditiveScene (SceneProperty property, string sceneName)
    {
        if (LoadSubSceneDictionary.Keys.Contains (sceneName) == true)
        {
            GameObject go = LoadSubSceneDictionary [sceneName];
            LoadSubSceneDictionary.Remove (sceneName);
            go.hideFlags = HideFlags.None;
            DestroyImmediate (go);
        }
    }

    private GameObject LoadWithCollection (string sceneName, string sceneFullName)
    {
        List<GameObject> listSceneObject = new List<GameObject> ();   
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject> ())
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            if (go.transform.parent != null)
                continue;
            listSceneObject.Add (go);
        }

        GameObject result = new GameObject (sceneName);
        listSceneObject.Add (result);

        EditorApplication.OpenSceneAdditive (sceneFullName);

        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject> ())
        {
            if (listSceneObject.Contains (go) == true)
                continue;
            if (go.hideFlags != HideFlags.None)
                continue;
            if (go.transform.parent != null)
                continue;

            // 複数あると都合がわるいものを破棄しておく
            if (go.GetComponent<EventSystem> () != null)
                DestroyImmediate (go.GetComponent<EventSystem> ());
            if (go.GetComponent<SceneProperty> () != null)
                DestroyImmediate (go.GetComponent<SceneProperty> ());

            go.transform.SetParent (result.transform);
        }

        return result;
    }
}
