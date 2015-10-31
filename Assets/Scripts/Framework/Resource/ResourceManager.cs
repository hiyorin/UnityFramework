using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Framework.Scene;
using Framework.Resource.Loader;

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
        private readonly Dictionary<string, BaseLoader> _dictLoader = new Dictionary<string, BaseLoader> ();
        private readonly List<BaseLoader> _listSelfLoader = new List<BaseLoader> ();

        public string assetBundleDomain { private set; get; }
        public AssetBundleManifest assetBundleManifest { private set; get; }

        private Dictionary<string, uint> _dictAssetBundleCRC = null;
        public uint GetAssetBundleCRC (string path)
        {
            uint crc = 0;
            if (_dictAssetBundleCRC != null)
                _dictAssetBundleCRC.TryGetValue (path, out crc);
            return crc;
        }

        protected override void OnInitialize ()
        {
            base.OnInitialize ();
            SceneManager.Instance.AddIgnoreCollection (gameObject.name);
            assetBundleDomain = "file://" + Application.streamingAssetsPath;

        }

        protected override void OnFinalize ()
        {
            base.OnFinalize ();
            SceneManager.Instance.RemoveIgnoreCollection (gameObject.name);
        }

        private bool IsCompleteLoader (BaseLoader loader)
        {
            if (loader.isComplete == false)
                return false;

            if (loader.isSuccessed == true)
            {
                CollectAssetFromLoader (loader);
            }
            else
            {
                Debug.LogError ("Error");
            }

            return true;
        }

        protected override void Update ()
        {
            base.Update ();

            int selfLoaderIndex = 0;
            while (_listSelfLoader.Count > selfLoaderIndex)
            {
                var loader = _listSelfLoader [selfLoaderIndex];
                loader.MoveNext ();
                if (IsCompleteLoader (loader) == true)
                {
                    _listSelfLoader.Remove (loader);
                }
                else
                {
                    selfLoaderIndex++;
                }
            }

            foreach (var loader in _dictLoader.Values)
            {
                loader.MoveNext ();
            }

            foreach (var requestSet in _dictRequestSet.Values)
            {
                if (requestSet.IsComplete () == true)
                    continue;

                int completeCount = 0;
                foreach (var request in requestSet.GetList ())
                {
                    BaseLoader loader = null;
                    if (_dictLoader.TryGetValue (request.url, out loader) == true)
                    {
                        if (loader.isComplete == false)
                            continue;
                        
                        if (loader.isSuccessed == true)
                        {
                            CollectAssetFromLoader (loader);
                        }
                        else
                        {
                            Debug.LogError (loader.error + "\n" + loader.path);
                        }
                        completeCount++;
                    }
                }

                if (requestSet.Count () <= completeCount)
                    requestSet.Stop ();
            }
        }

        private void CollectAssetFromLoader (BaseLoader loader)
        {
            if (loader is AssetBundleLoader)
            {
                if (_dictResourceAssetBundle.ContainsKey (loader.path) == true)
                    return;
                AssetBundleLoader assetBundleLoader = loader as AssetBundleLoader;
                if (assetBundleLoader.asset != null)
                    _dictResourceAssetBundle.Add (loader.path, new ResourceItem (assetBundleLoader.asset));
                foreach (var sub in assetBundleLoader.assetDependencies)
                {
                    if (sub.asset != null)
                        _dictResourceAsset.Add (sub.path, new ResourceItem (sub.asset));
                }
            }
            else if (loader is AssetBundleManifestLoader)
            {
                assetBundleManifest = ((AssetBundleManifestLoader)loader).assetBundleManifest;
            }
            else if (loader is AssetBundleCRCLoader)
            {
                _dictAssetBundleCRC = ((AssetBundleCRCLoader)loader).crcDict;
            }
            else if (loader is TextureLoader)
            {
                if (_dictResourceTexture.ContainsKey (loader.path) == true)
                    return;
                ResourceItem r = new ResourceItem (((TextureLoader)loader).texture);
                _dictResourceTexture.Add (loader.path, r);
            }
            else if (loader is AssetLoader)
            {
                if (_dictResourceAsset.ContainsKey (loader.path) == true)
                    return;
                ResourceItem r = new ResourceItem (((AssetLoader)loader).asset);
                _dictResourceAsset.Add (loader.path, r);
            }
            else
            {
                Debug.LogErrorFormat ("Loader Error {0}", loader.GetType ());
            }
        }

        public void LoadAssetBundleManifest ()
        {
            assetBundleManifest = null;
            BaseLoader loader = BaseLoader.Init<AssetBundleManifestLoader> ("AssetBundleManifest");
            _listSelfLoader.Add (loader);
        }

        public bool IsAssetBundleManifestExists ()
        {
            return assetBundleManifest != null;
        }

        public void LoadAssetBundleCRC ()
        {
            _dictAssetBundleCRC = null;
            BaseLoader loader = BaseLoader.Init<AssetBundleCRCLoader> ("AssetBundleCRC");
            _listSelfLoader.Add (loader);
        }

        public bool IsAssetBundleCRCExists ()
        {
            return _dictAssetBundleCRC != null;
        }

        public bool RegisterRequestSet (string label, ResourceRequestSet resourceSet)
        {
            if (_dictRequestSet.ContainsKey (label) == true)
                return false;

            foreach (var requestItem in resourceSet.GetList ())
            {
                BaseLoader loader = null;
                if (_dictLoader.ContainsKey (requestItem.url) == true)
                    continue;
                
                ResourceItem resourceItem = null;
                switch (requestItem.type)
                {
                case ResourceType.Asset:
                    if (_dictResourceAsset.TryGetValue (requestItem.url, out resourceItem) == false)
                        loader = BaseLoader.Init<AssetLoader> (requestItem.url);
                    break;
                case ResourceType.AssetBundle:
                    if (_dictResourceAssetBundle.TryGetValue (requestItem.url, out resourceItem) == false)
                        loader = BaseLoader.Init<AssetBundleLoader> (requestItem.url);
                    break;
                case ResourceType.Texture:
                    if (_dictResourceTexture.TryGetValue (requestItem.url, out resourceItem) == false)
                        loader = BaseLoader.Init<TextureLoader> (requestItem.url);
                    break;
                default:
                    Debug.LogErrorFormat ("{0} {1} NotFound {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, requestItem.type);
                    break;
                }

                if (resourceItem != null)
                    resourceItem.IncRef ();
                if (loader != null)
                    _dictLoader.Add (requestItem.url, loader);
            }
            _dictRequestSet.Add (label, resourceSet);
            return true;
        }

        public bool UnregisterRequestSet (string label)
        {
            ResourceRequestSet requestSet = null;
            if (_dictRequestSet.TryGetValue (label, out requestSet) == false)
                return false;

            requestSet.Stop ();

            foreach (var request in requestSet.GetList ())
            {
                switch (request.type)
                {
                case ResourceType.Asset:
                    RemoveAsset (request.url);
                    break;
                case ResourceType.AssetBundle:
                    RemoveAssetBundle (request.url);
                    foreach (var dependency in assetBundleManifest.GetDirectDependencies (request.url))
                        RemoveAssetBundle (dependency);
                    break;
                case ResourceType.Texture:
                    RemoveTexture (request.url);
                    break;
                default:
                    Debug.LogErrorFormat ("{0} {1} NotFound {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, request.type);
                    break;
                }
            }

            _dictRequestSet.Remove (label);
            Resources.UnloadUnusedAssets ();
            return true;
        }

        private void RemoveAsset (string path)
        {
            ResourceItem res = null;
            if (_dictResourceAsset.TryGetValue (path, out res) == false)
            {
                Debug.LogErrorFormat ("RemoveAsset not loaded path={0}", path);
                return;
            }

            RemoveResource (path, res);
        }

        private void RemoveAssetBundle (string path)
        {
            ResourceItem res = null;
            if (_dictResourceAssetBundle.TryGetValue (path, out res) == false)
            {
                Debug.LogErrorFormat ("RemoveAssetBundle not loaded path={0}", path);
                return;
            }

            RemoveResource (path, res);
        }

        private void RemoveTexture (string path)
        {
            ResourceItem res = null;
            if (_dictResourceTexture.TryGetValue (path, out res) == false)
            {
                Debug.LogErrorFormat ("RemoveTexture not loaded path={0}", path);
                return;
            }

            RemoveResource (path, res);
        }

        private void RemoveResource (string path, ResourceItem res)
        {
            res.DecRef ();
            if (res.referenceCount > 0)
                return;

            _dictResourceAssetBundle.Remove (path);
        }

        private bool IsLoadedExist (string url, ResourceType type)
        {
            switch (type)
            {
            case ResourceType.Asset:
                return _dictResourceAsset.ContainsKey (url);
            case ResourceType.AssetBundle:
                return _dictResourceAssetBundle.ContainsKey (url);
            case ResourceType.Texture:
                return _dictResourceTexture.ContainsKey (url);
            default:
                Debug.LogErrorFormat ("{0} {1} NotFound {2}", typeof(ResourceManager).Name, MethodBase.GetCurrentMethod ().Name, type);
                return false;
            }
        }

        public bool IsAssetExist (string url)
        {
            return IsLoadedExist (url, ResourceType.Asset);
        }

        public bool IsAssetBundleExist (string url)
        {
            return IsLoadedExist (url, ResourceType.AssetBundle);
        }

        public bool IsTextureExist (string url)
        {
            return IsLoadedExist (url, ResourceType.Texture);
        }

        public bool Contains (string path, ResourceType type)
        {
            if (IsLoadedExist (path, type) == true)
                return true;

            BaseLoader loader = null;
            if (_dictLoader.TryGetValue (path, out loader) == true)
                return true;

            return loader.Contains (path);
        }

        public Object GetAsset (string url)
        {
            ResourceItem resource = null;
            if (_dictResourceAsset.TryGetValue (url, out resource) == false)
                return null;
            return resource.resource;
        }

        public Object GetAssetBundle (string path)
        {
            ResourceItem resource = null;
            if (_dictResourceAssetBundle.TryGetValue (path, out resource) == false)
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