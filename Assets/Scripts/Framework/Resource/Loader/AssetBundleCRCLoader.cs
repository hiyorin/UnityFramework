using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Framework.Resource.Loader
{
    public class AssetBundleCRCLoader : BaseLoader
    {
        public Dictionary<string, uint> crcDict { private set; get; }

        protected override IEnumerator GenerateLoadProcess (string path)
        {
            string target = GetPlatformFolderForAssetBundles ();
            string baseUrl = Path.Combine (ResourceManager.Instance.assetBundleDomain, target);

            string crcUrl = Path.Combine (baseUrl, ResourceManagerSettings.CRCFileName);
            using (WWW www = new WWW (crcUrl))
            {
                yield return www;
                while (www.progress < 1.0f || www.isDone == false)
                {
                    yield return null;
                }
                if (string.IsNullOrEmpty (www.text) == true)
                {
                    Failed (www.error);
                    yield break;
                }

                crcDict = JsonMapper.ToObject<Dictionary<string, uint>> (www.text);
                if (crcDict == null)
                {
                    Failed ("JSON Parse Error");
                    yield break;
                }

                Successed ();
            }
        }
    }
}