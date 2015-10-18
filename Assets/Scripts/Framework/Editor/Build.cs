using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Build
{
    private static readonly Dictionary<BuildTarget, string> Extension = new Dictionary<BuildTarget, string> {
        { BuildTarget.Android,   "apk" },
        { BuildTarget.iOS,       "ipa" },
    };

    [MenuItem ("Tools/Build/Build Player (Develop)")]
    private static void BuildPlayerDevelop ()
    {
        string extension = string.Empty;
        Extension.TryGetValue (EditorUserBuildSettings.activeBuildTarget, out extension);
        string locationPathName = EditorUtility.SaveFilePanel (
            "build develop", "", PlayerSettings.productName, extension);
        if (string.IsNullOrEmpty (locationPathName) == true)
            return;
        BuildAllAssetBundlesDevelop ();
        BuildPlayer (locationPathName, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.Development);
    }

    [MenuItem ("Tools/Build/Build and Run Player (Develop)")]
    private static void BuildAndRunPlayerDevelop ()
    {
        string extension = string.Empty;
        Extension.TryGetValue (EditorUserBuildSettings.activeBuildTarget, out extension);
        string locationPathName = EditorUtility.SaveFilePanel (
            "build & run develop", "", PlayerSettings.productName, extension);
        if (string.IsNullOrEmpty (locationPathName) == true)
            return;
        BuildAllAssetBundlesProduction ();
        BuildPlayer (locationPathName, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.Development | BuildOptions.AutoRunPlayer);
    }

    [MenuItem ("Tools/Build/Build Player (Production)")]
    private static void BuildPlayerProduction ()
    {
        string extension = string.Empty;
        Extension.TryGetValue (EditorUserBuildSettings.activeBuildTarget, out extension);
        string locationPathName = EditorUtility.SaveFilePanel (
            "build production", "", PlayerSettings.productName, extension);
        if (string.IsNullOrEmpty (locationPathName) == true)
            return;
        BuildPlayer (locationPathName, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None);
    }

    [MenuItem ("Tools/Build/Build and Run Player (Production)")]
    private static void BuildAndRunPlayerProduction ()
    {
        string extension = string.Empty;
        Extension.TryGetValue (EditorUserBuildSettings.activeBuildTarget, out extension);
        string locationPathName = EditorUtility.SaveFilePanel (
            "build & run production", "", PlayerSettings.productName, extension);
        if (string.IsNullOrEmpty (locationPathName) == true)
            return;
        BuildPlayer (locationPathName, EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None | BuildOptions.AutoRunPlayer);
    }

    [MenuItem ("Tools/Build/Build AssetBundles (Develop)")]
    private static void BuildAllAssetBundlesDevelop ()
    {
        string outputPath = ResourceManagerSettings.StancaloneAssetBundleOutputPath;
        BuildAllAssetBundles (outputPath);
    }

    [MenuItem ("Tools/Build/Build AssetBundles (Production)")]
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

    private static void BuildAndroid ()
    {
        BuildPlayer (PlayerSettings.productName + "." + Extension [BuildTarget.Android], BuildTarget.Android, BuildOptions.None);
    }

    private static void BuildIOS ()
    {
        BuildPlayer (PlayerSettings.productName + "." + Extension [BuildTarget.iOS], BuildTarget.iOS, BuildOptions.None);
    }

    private static void BuildPlayer (string locationPathName, BuildTarget buildTarget, BuildOptions buildOptions)
    {
        List<string> listSceneName = new List<string> ();
        foreach (var scene in SceneManagerSettings.ListBuidlInScene)
        {
            if (scene == null)
                continue;
            listSceneName.Add (AssetDatabase.GetAssetPath (scene));
        }
        BuildPipeline.BuildPlayer (listSceneName.ToArray (), locationPathName,
            buildTarget, buildOptions | BuildOptions.Il2CPP);
        Debug.Log ("Build Player Complete");
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
        Debug.Log ("Build AssetBundle Complete");
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