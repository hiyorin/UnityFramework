using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Framework.Scene;

namespace Framework.Resource
{
    public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
    {
        #if UNITY_EDITOR
        public abstract class AbstractEditor : UnityEditor.Editor {
            public ResourceManager instance { get { return target as ResourceManager; } }
            public Dictionary<string, ResourceRequestSet> dictRequestSet { get { return instance._dictRequestSet; } }
            public Dictionary<string, ResourceItem> dictResourceAsset { get { return instance._dictResourceAsset; } }
            public Dictionary<string, ResourceItem> dictResourceAssetBundle { get { return instance._dictResourceAssetBundle; } }
            public Dictionary<string, ResourceItem> dictResourceTexture { get { return instance._dictResourceTexture; } }
        }
        #endif

        private readonly Dictionary<string, ResourceRequestSet> _dictRequestSet = new Dictionary<string, ResourceRequestSet> ();
        private readonly Dictionary<string, ResourceItem> _dictResourceAsset = new Dictionary<string, ResourceItem> ();
        private readonly Dictionary<string, ResourceItem> _dictResourceAssetBundle = new Dictionary<string, ResourceItem> ();
        private readonly Dictionary<string, ResourceItem> _dictResourceTexture = new Dictionary<string, ResourceItem> ();

        private IEnumerator _taskLoader = null;
        private ResourceRequestSet _currentLoadRequest = null;

        protected override void OnInitialize ()
        {
            base.OnInitialize ();
            SceneManager.Instance.AddIgnoreCollection (gameObject.name);
        }

        protected override void OnFinalize ()
        {
            base.OnFinalize ();
            SceneManager.Instance.RemoveIgnoreCollection (gameObject.name);
        }

        protected override void Update ()
        {
            base.Update ();
            if (_taskLoader == null)
            {
                foreach (var requestSet in _dictRequestSet.Values)
                {
                    if (requestSet.IsComplete () == true)
                        continue;

                    _currentLoadRequest = requestSet;
                    _taskLoader = LoadResourceProcess (requestSet);
                    break;
                }
            }
            else
            {
                if (_taskLoader.MoveNext () == true)
                    return;

                _taskLoader = null;
                _currentLoadRequest = null;
            }
        }

        private IEnumerator LoadResourceProcess (ResourceRequestSet requestSet)
        {
            if (requestSet.IsComplete () == true)
                yield break;
            
            foreach (var requestItem in requestSet.GetList ())
            {
                IEnumerator load = null;
                switch (requestItem.type)
                {
                case ResourceType.Asset:
                    ResourceItem asset = null;
                    if (_dictResourceAsset.TryGetValue (requestItem.url, out asset) == true)
                        asset.IncRef ();
                    else
                        load = LoadAsset (requestItem.url);
                    break;
                case ResourceType.AssetBundle:

                    break;
                case ResourceType.Texture:
                    ResourceItem texture = null;
                    if (_dictResourceTexture.TryGetValue (requestItem.url, out texture) == true)
                        texture.IncRef ();
                    else
                        load = LoadTexture (requestItem.url);
                    break;
                default:
                    Debug.LogErrorFormat ("{0} {1} NotFound {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, requestItem.type);
                    break;
                }
                if (load != null)
                    while (load.MoveNext () == true)
                        yield return null;
            }

            requestSet.Stop ();
        }

        /// <summary>
        /// リクエストの登録
        /// </summary>
        /// <returns><c>true</c>, if request set was registered, <c>false</c> otherwise.</returns>
        /// <param name="label">Label.</param>
        /// <param name="resourceSet">Resource set.</param>
        public bool RegisterRequestSet (string label, ResourceRequestSet resourceSet)
        {
            if (_dictRequestSet.ContainsKey (label) == true) {
                return false;
            }
            _dictRequestSet.Add (label, resourceSet);
            return true;
        }

        /// <summary>
        /// リクエストの削除
        /// </summary>
        /// <returns><c>true</c>, if request set was unregistered, <c>false</c> otherwise.</returns>
        /// <param name="label">Label.</param>
        public bool UnregisterRequestSet (string label)
        {
            ResourceRequestSet requestSet = null;
            if (_dictRequestSet.TryGetValue (label, out requestSet) == false)
                return false;

            requestSet.Stop ();
            if (requestSet.Equals (_currentLoadRequest) == true)
            {
                _taskLoader = null;
                _currentLoadRequest = null;
            }

            foreach (var request in requestSet.GetList ())
            {
                Dictionary<string, ResourceItem> dict = null;
                switch (request.type)
                {
                case ResourceType.Asset:
                    dict = _dictResourceAsset;
                    break;
                case ResourceType.AssetBundle:
                    dict = _dictResourceAssetBundle;
                    break;
                case ResourceType.Texture:
                    dict = _dictResourceTexture;
                    break;
                default:
                    Debug.LogErrorFormat ("{0} {1} NotFound {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, request.type);
                    break;
                }

                ResourceItem resource = null;
                if (dict.TryGetValue (request.url, out resource) == false)
                    continue;
                
                resource.DecRef ();
                if (resource.referenceCount > 0)
                    continue;

                Object.Destroy (resource.resource);
                dict.Remove (request.url);
            }

            _dictRequestSet.Remove (label);
            return true;
        }

        private IEnumerator LoadAsset (string url)
        {
            ResourceRequest request = Resources.LoadAsync (url);
            request.allowSceneActivation = false;
            while (request.progress < 0.9f && request.isDone == false)
                yield return null;
            request.allowSceneActivation = true;

            if (request.asset == null)
            {
                Debug.LogErrorFormat ("{0} {1} Error {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, url);
                yield break;
            }

            ResourceItem resource = new ResourceItem (request.asset);
            _dictResourceAsset.Add (url, resource);
        }

        private IEnumerator LoadTexture (string url)
        {
            WWW www = new WWW (url);
            yield return www;

            while (www.progress < 1.0f || www.isDone == false)
                yield return null;
            
            if (www.texture == null)
            {
                Debug.LogErrorFormat ("{0} {1} Error {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, url);
                yield break;
            }

            ResourceItem resource = new ResourceItem (www.texture);
            _dictResourceTexture.Add (url, resource);
        }

        public Object GetAsset (string url)
        {
            ResourceItem resource = null;
            if (_dictResourceAsset.TryGetValue (url, out resource) == false)
                return null;
            return resource.resource;
        }

        public Texture2D GetTexture (string url)
        {
            ResourceItem resource = null;
            if (_dictResourceTexture.TryGetValue (url, out resource) == false)
                return null;
            return resource.resource as Texture2D;
        }
    }
}