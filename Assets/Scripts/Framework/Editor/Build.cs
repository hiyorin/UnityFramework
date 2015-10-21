using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Framework;
using LitJson;

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
        // Check Output Directory
        string targetFolderName = EditorUserBuildSettings.activeBuildTarget.ToString ();
        string properPath = Path.Combine (outputPath, targetFolderName);
        if (FileUtility.CreateDirectory (properPath) == false)
        {
            Debug.LogErrorFormat ("Output Directory Error path={0}", properPath);
            return;
        }

        // Check Source Directory
        string sourceDirectory = ResourceManagerSettings.AssetBundleSourceDirectory;
        if (Directory.Exists (sourceDirectory) == false)
        {
            Debug.LogErrorFormat ("Source Directory Error path={0}", sourceDirectory);
            return;
        }

        // Create Build Asset List
        List<AssetBundleBuild> listAssetBunle = new List<AssetBundleBuild> ();
        foreach (var assetName in AssetDatabase.GetAllAssetPaths ())
        {
            if (assetName.StartsWith (sourceDirectory) == false)
                continue;
            if (assetName.EndsWith (".cs") == true || assetName.EndsWith (".js") == true || assetName.EndsWith (".boo") == true)
                continue;
            if (AssetDatabase.IsValidFolder (assetName) == true)
                continue;

            string assetBundleName = assetName.Substring (sourceDirectory.Length, assetName.LastIndexOf ('.') - sourceDirectory.Length);
            AssetBundleBuild build = new AssetBundleBuild ();
            build.assetBundleName = assetBundleName;
            build.assetNames = new string[] { assetName };

            listAssetBunle.Add (build);
            Debug.LogFormat ("Build Asset : {0}", assetBundleName);
        }

        // Create AssetBundles
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles (
            properPath,
            listAssetBunle.ToArray (),
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget
        );
        Debug.Log ("Build AssetBundle Complete");

        // Create CRC File
        Dictionary<string, uint> dictCRC = new Dictionary<string, uint> ();
        foreach (var assetBundle in manifest.GetAllAssetBundles ())
        {
            uint crc = 0;
            if (BuildPipeline.GetCRCForAssetBundle (Path.Combine (properPath, assetBundle), out crc))
            {
                dictCRC.Add (assetBundle, crc);
                Debug.LogFormat ("CRC : {0} {1}", crc, assetBundle);
            }
        }

        string json = JsonMapper.ToJson (dictCRC);
        using (StreamWriter stream = File.CreateText (Path.Combine (properPath, ResourceManagerSettings.CRCFileName)))
        {
            stream.Write (json);
            stream.Close ();
        }
        Debug.Log ("Created CRD Json Complete");
    }
}