using UnityEngine;
using System.IO;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class AssetBundleLoader : BaseLoader
    {
        public Object asset { private set; get; }

        protected override IEnumerator GenerateLoadProcess (string path)
        {
            if (Caching.ready == false)
                yield return null;
            
            string url = Path.Combine (ResourceManager.Instance.assetBundleDomain, GetPlatformFolderForAssetBundles ());
            url = Path.Combine (url, path);
            Hash128 hash = ResourceManager.Instance.assetBundleManifest.GetAssetBundleHash (path);
            uint crc = ResourceManager.Instance.GetAssetBundleCRC (path);

            if (hash.ToString ().Equals ("00000000000000000000000000000000") == true)
            {
                Failed ("That AssetBundle does not in the manifest.");
                yield break;
            }

            if (crc == 0)
            {
                Failed ("CRC is zero");
                yield break;
            }

            using (WWW www = WWW.LoadFromCacheOrDownload (url, hash, crc))
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

                AssetBundleRequest async = www.assetBundle.LoadAssetAsync (path);
                while (async.progress < 0.9f || async.isDone == false)
                {
                    yield return null;
                }
                if (async.asset == null)
                {
                    Failed ("AssetBundle.LoadAssetAsync");
                    yield break;
                }

                asset = async.asset;
                www.assetBundle.Unload (true);
                Successed ();
            }
        }
    }
}