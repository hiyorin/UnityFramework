using UnityEngine;
using System.IO;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class AssetBundleDependencyLoader : BaseLoader
    {
        public Object asset { protected set; get; }

        public string GetURL ()
        {
            string url = Path.Combine (ResourceManager.Instance.assetBundleDomain, GetPlatformFolderForAssetBundles ());
            url = Path.Combine (url, path);
            return url;
        }

        protected override IEnumerator GenerateLoadProcess ()
        {
            while (Caching.ready == false)
                yield return null;

            DownloadSceneManager.Instance.Add (this);

            while (ResourceManager.Instance.IsAssetBundleManifestExists () == false)
                yield return null;

            string url = GetURL ();
            Hash128 hash = ResourceManager.Instance.assetBundleManifest.GetAssetBundleHash (path);

            if (hash.ToString ().Equals ("00000000000000000000000000000000") == true)
            {
                Failed ("That AssetBundle does not in the manifest.");
                yield break;
            }

            while (ResourceManager.Instance.IsAssetBundleCRCExists () == false)
                yield return null;

            uint crc = ResourceManager.Instance.GetAssetBundleCRC (path);
            if (crc == 0)
            {
                Failed ("CRC is zero");
                yield break;
            }

            using (WWW www = WWW.LoadFromCacheOrDownload (url, hash, crc))
            {
                while (www.progress < 1.0f || www.isDone == false)
                {
                    progress = www.progress;
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
                www.assetBundle.Unload (false);
                Successed ();
            }
        }
    }
}