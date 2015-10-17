using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class SceneManagerSettings : ScriptableObject
{
	private const string SettingsAssetName = "SceneManagerSettings";
	private const string SettingsAssetPath = "Resources";
	private const string SettingsAssetExtension = ".asset";

    private static SceneManagerSettings instance;

	private static SceneManagerSettings Instance ()
	{
		if (instance == null)
		{
			instance = Resources.Load (SettingsAssetName) as SceneManagerSettings;
			if (instance == null)
			{
				instance = CreateInstance<SceneManagerSettings> ();
#if UNITY_EDITOR
				string properPath = Path.Combine (Application.dataPath, SettingsAssetPath);
				if (Directory.Exists (properPath) == false)
					AssetDatabase.CreateFolder ("Assets", SettingsAssetPath);

				string fullPath = Path.Combine (Path.Combine("Assets", SettingsAssetPath),
					SettingsAssetName + SettingsAssetExtension
				);
				AssetDatabase.CreateAsset (instance, fullPath);
#endif
			}
		}
		return instance;
	}

#if UNITY_EDITOR
	[MenuItem ("Tools/SceneManagerSettings")]
	public static void Edit ()
	{
		Selection.activeObject = Instance ();
	}

	private static void DirtyEditor ()
	{
		EditorUtility.SetDirty(Instance ());
	}
#endif

    [Header ("Screen Settings")]
    [SerializeField, Label ("Width")]
    private uint _screenWidth = 1080;
    [SerializeField, Label ("Height")]
    private uint _screenHeight = 1920;
    [SerializeField, LabelRange ("MacthWidthOrHeight", 0.0f, 1.0f)]
    private float _screenMatchWidthOrHeight = 0.0f;

    [Header ("Build Settings"), Space]
    [SerializeField, FilterFileExtension ("unity")]
    private DefaultAsset[] _listBuildInScene = new DefaultAsset[] {};

    [Space]
    [SerializeField]
    private string[] _listCollectionIgnoreObjectName = new string[] {};
    [SerializeField]
    private string[] _listStaticSceneName = new string[] {};
    [SerializeField]
    private string[] _listResidentSceneName = new string[] {};
    [SerializeField]
    private string[] _listFirstLoadTaskSceneName = new string[] {};

    public static uint ScreenWidth {
        get { return Instance ()._screenWidth; }
    }

    public static uint ScreenHeight {
        get { return Instance ()._screenHeight; }
    }

    public static float ScreenMatchWidthOrHeight {
        get { return Instance ()._screenMatchWidthOrHeight; }
    }

    public static DefaultAsset[] ListBuildInScene {
        get { return Instance ()._listBuildInScene; }
    }

	public static string[] ListCollectionIgnoreObjectName {
        get { return Instance ()._listCollectionIgnoreObjectName; }
	}

	public static string[] ListStaticSceneName {
        get { return Instance ()._listStaticSceneName; }
	}

    public static string[] ListResidentSceneName {
        get { return Instance ()._listResidentSceneName; }
    }

	public static string[] ListFirstLoadTaskSceneName {
        get { return Instance ()._listFirstLoadTaskSceneName; }
	}
}
