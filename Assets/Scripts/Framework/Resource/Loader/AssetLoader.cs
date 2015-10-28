using UnityEngine;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class AssetLoader : BaseLoader
    {
        public Object asset { private set; get; }

        protected override IEnumerator GenerateLoadProcess ()
        {
            ResourceRequest request = Resources.LoadAsync (path);
            request.allowSceneActivation = false;
            while (request.progress < 0.9f && request.isDone == false)
            {
                progress = request.progress;
                yield return null;
            }
            request.allowSceneActivation = true;

            if (request.asset == null)
            {
                Failed ("NotFound");
                yield break;
            }

            asset = request.asset;
            Successed ();
        }
    }
}