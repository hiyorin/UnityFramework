using UnityEngine;
using UnityEditor;
using System;
using UniLinq;
using Framework.Scene;

[CustomEditor (typeof (SceneManagerSettings))]
public class SceneManagerSettingsInspector : Editor
{
	private SceneManagerSettings instance;

	public override void OnInspectorGUI ()
	{
		instance = (SceneManagerSettings)target;

        Screen ();
        EditorGUILayout.Space ();
        CollectionIgnoreObjectName ();
        EditorGUILayout.Space ();
        StaticSceneName ();
        EditorGUILayout.Space ();
        ResidentSceneName ();
        EditorGUILayout.Space ();
		FirstLoadTaskSceneName ();
	}

    private void Screen ()
    {
        EditorGUILayout.LabelField ("Screen");
        EditorGUILayout.BeginVertical ("box");

        Vector2 screenSize = EditorGUILayout.Vector2Field ("Size", new Vector2 (instance.screenWidth, instance.screenHeight));
        instance.screenWidth = (uint)screenSize.x;
        instance.screenHeight = (uint)screenSize.y;

        instance.screenMatchWidthOrHeight = EditorGUILayout.Slider ("MatchWidthOrHeight", instance.screenMatchWidthOrHeight, 0.0f, 1.0f);

        EditorGUILayout.EndVertical ();
    }

	private void CollectionIgnoreObjectName ()
	{
        EditorGUILayout.LabelField ("CollectionIgnoreObjectName");
		EditorGUILayout.BeginVertical ("box");
		foreach (var row in instance.listCollectionIgnoreObjectName.Select ((name, index) => new {name, index}))
			instance.listCollectionIgnoreObjectName [row.index] = EditorGUILayout.TextField (row.index.ToString (), row.name);
        
		string[] resize = instance.listCollectionIgnoreObjectName;
		ResizeArray (ref resize);
		instance.listCollectionIgnoreObjectName = resize;

		EditorGUILayout.EndVertical ();
	}

	private void StaticSceneName ()
	{
        EditorGUILayout.LabelField ("StaticSceneName");
		EditorGUILayout.BeginVertical ("box");
        foreach (var row in instance.listStaticSceneName.Select ((name, index) => new {name, index}))
            instance.listStaticSceneName [row.index] = EditorGUILayout.TextField (row.index.ToString (), row.name);

		string[] resize = instance.listStaticSceneName;
		ResizeArray (ref resize);
		instance.listStaticSceneName = resize;

		EditorGUILayout.EndVertical ();
	}

    private void ResidentSceneName ()
    {
        EditorGUILayout.LabelField ("ResidentSceneName");
        EditorGUILayout.BeginVertical ("box");
        foreach (var row in instance.listResidentSceneName.Select ((name, index) => new {name, index}))
            instance.listResidentSceneName [row.index] = EditorGUILayout.TextField (row.index.ToString (), row.name);

        string[] resize = instance.listResidentSceneName;
        ResizeArray (ref resize);
        instance.listResidentSceneName = resize;

        EditorGUILayout.EndVertical ();
    }

	private void FirstLoadTaskSceneName ()
	{
        EditorGUILayout.LabelField ("FirstLoadTaskSceneName");
		EditorGUILayout.BeginVertical ("box");
		foreach (var row in instance.listFirstLoadTaskSceneName.Select ((name, index) => new {name, index}))
			instance.listFirstLoadTaskSceneName [row.index] = EditorGUILayout.EnumPopup (
				row.index.ToString (), (Scene)Enum.Parse (typeof(Scene), row.name))
				.ToString ();

		string[] resize = instance.listFirstLoadTaskSceneName;
		ResizeArray (ref resize);
		instance.listFirstLoadTaskSceneName = resize;

		EditorGUILayout.EndVertical ();
	}

	private void ResizeArray (ref string[] array)
	{
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+") == true)
			Array.Resize (ref array, array.Length + 1);
		if (GUILayout.Button ("-") == true)
			Array.Resize (ref array, array.Length - 1);
		EditorGUILayout.EndHorizontal ();
	}
}
