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

        protected override IEnumerator GenerateLoadProcess ()
        {
            string srcDir = ResourceManagerSettings.AssetBundleSourceDirectory;
            string localPath = Path.Combine (srcDir, path);

            asset = AssetDatabase.LoadMainAssetAtPath (localPath);
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