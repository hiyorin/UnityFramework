using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem ("Tools/Resource/Build AssetBundles (Standalone)")]
    private static void BuildAllAssetBundlesStandalone ()
    {
        string outputPath = ResourceManagerSettings.StancaloneAssetBundleOutputPath;
        BuildAllAssetBundles (outputPath);
    }

    [MenuItem ("Tools/Resource/Build AssetBundles (Production)")]
    private static void BuildAllAssetBundlesProduction ()
    {
        string outputPath = ResourceManagerSettings.ProductionAssetBundleOutputPath;
        BuildAllAssetBundles (outputPath);
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
}