using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;

[InitializeOnLoad]
#endif
public class ResourceManagerSettings : ScriptableObject
{
    private const string SettingsAssetName = "ResourceManagerSettings";
    private const string SettingsAssetPath = "Resources";
    private const string SettingsAssetExtension = ".asset";

    private static ResourceManagerSettings instance;

    private static ResourceManagerSettings Instance ()
    {
        if (instance == null)
        {
            instance = Resources.Load (SettingsAssetName) as ResourceManagerSettings;
            if (instance == null)
            {
                instance = CreateInstance<ResourceManagerSettings> ();
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
    [MenuItem ("Tools/Resource/Settings")]
    public static void Edit ()
    {
        Selection.activeObject = Instance ();
    }

    private static void DirtyEditor ()
    {
        EditorUtility.SetDirty(Instance ());
    }
#endif

    [SerializeField, Header ("AssetBundle")]
    private string _standaloneAssetBundleOutputPath = "Assets/StreamingAssets/";
    public static string StancaloneAssetBundleOutputPath { get { return Instance ()._standaloneAssetBundleOutputPath; } }

    [SerializeField]
    private string _productionAssetBundleOutoutPath = "AssetBundles/";
    public static string ProductionAssetBundleOutputPath { get { return Instance ()._productionAssetBundleOutoutPath; } }
}
