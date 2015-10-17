using UnityEditor;
using UnityEngine;
using System.IO;

public class Build
{
    [MenuItem ("Tools/Build/AssetBundles (Standalone)")]
    private static void BuildAllAssetBundlesStandalone ()
    {
        string outputPath = ResourceManagerSettings.StancaloneAssetBundleOutputPath;
        BuildAllAssetBundles (outputPath);
    }

    [MenuItem ("Tools/Build/AssetBundles (Production)")]
    private static void BuildAllAssetBundlesProduction ()
    {
        string outputPath = ResourceManagerSettings.ProductionAssetBundleOutputPath;
        BuildAllAssetBundles (outputPath);
    }

    [MenuItem ("Tools/Build/Draw Build AssetBindles")]
    private static void DrawBuildAllAssetBundles ()
    {
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames ();
        for (var i = 0; i < assetBundleNames.Length; i++)
            Debug.LogFormat ("{0} : {1}", i.ToString ("000"), assetBundleNames [i]);
    }

    private static void BuildAllAssetBundles (string outputPath)
    {
        string targetFolderName = EditorUserBuildSettings.activeBuildTarget.ToString ();

        string properPath = Path.Combine (outputPath, targetFolderName);
        string[] splitPath = properPath.Split (Path.PathSeparator);
        string path = string.Empty;
        foreach (var row in splitPath)
        {
            path = Path.Combine (path, row);
            if (Directory.Exists (path) == false)
                Directory.CreateDirectory (path);
        }

        BuildPipeline.BuildAssetBundles (
            properPath,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget
        );
    }

//    [MenuItem ("Tools/Test")]
//    private static void Test ()
//    {
//        foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames ())
//        {
//            foreach (var assetName in AssetDatabase.GetAssetPathsFromAssetBundle (assetBundleName))
//            {
//                Debug.Log (assetBundleName + " : " + assetName);
//            }
//        }
//    }
}