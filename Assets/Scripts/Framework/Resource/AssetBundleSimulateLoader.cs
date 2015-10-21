#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class AssetBundleSimulateLoader : BaseLoader
    {
        public Object asset { private set; get; }

        protected override IEnumerator GenerateLoadProcess (string path)
        {
            string srcDir = ResourceManagerSettings.AssetBundleSourceDirectory;
            path = Path.Combine (srcDir, path);

            asset = AssetDatabase.LoadMainAssetAtPath (path);
            if (asset == null)
            {
                Failed ("NotFound");
                yield break;
            }

            Successed ();
        }
    }
}
#endif