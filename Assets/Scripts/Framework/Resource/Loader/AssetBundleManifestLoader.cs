using UnityEngine;
using System.IO;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class AssetBundleManifestLoader : BaseLoader
    {
        public AssetBundleManifest assetBundleManifest { private set; get; }
        protected override IEnumerator GenerateLoadProcess (string path)
        {
            string target = GetPlatformFolderForAssetBundles ();
            string baseUrl = Path.Combine (ResourceManager.Instance.assetBundleDomain, target);

            string manifestUrl = Path.Combine (baseUrl, target);
            using (WWW www = new WWW (manifestUrl))
            {
                yield return www;
                while (www.progress < 1.0f || www.isDone == false)
                {
                    yield return null;
                }
                if (www.assetBundle == null)
                {
                    Failed (www.error);
                    yield break;
                }

                AssetBundleRequest async = www.assetBundle.LoadAssetAsync ("AssetBundleManifest");
                while (async.progress < 0.9f || async.isDone == false)
                {
                    yield return null;
                }
                if (async.asset == null)
                {
                    Failed ("AssetBundle.LoadAssetAsync");
                    yield break;
                }

                assetBundleManifest = async.asset as AssetBundleManifest;
                Successed ();
            }
        }
    }
}