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

        protected override void Update ()
        {
            base.Update ();

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
                            Debug.LogError ("Error");
                        }

                        _dictLoader.Remove (request.url);
                    }

                    completeCount++;
                }

                if (requestSet.Count () <= completeCount)
                    requestSet.Stop ();
            }
        }

        private void CollectAssetFromLoader (BaseLoader loader)
        {
            if (loader is AssetBundleLoader)
            {
                ResourceItem r = new ResourceItem (((AssetBundleLoader)loader).asset);
                _dictResourceAsset.Add (loader.path, r);
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
                ResourceItem r = new ResourceItem (((TextureLoader)loader).texture);
                _dictResourceTexture.Add (loader.path, r);
            }
            else if (loader is AssetLoader)
            {
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
            _dictLoader.Add ("AssetBundleManifest", loader);
        }

        public void LoadAssetBundleCRC ()
        {
            _dictAssetBundleCRC = null;
            BaseLoader loader = BaseLoader.Init<AssetBundleCRCLoader> ("AssetBundleCRC");
            _dictLoader.Add ("AssetBundleCRC", loader);
        }

        /// <summary>
        /// リクエストの登録
        /// </summary>
        /// <returns><c>true</c>, if request set was registered, <c>false</c> otherwise.</returns>
        /// <param name="label">Label.</param>
        /// <param name="resourceSet">Resource set.</param>
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
                    RegisterRequestAssetBundle (requestItem.url);
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

        private bool RegisterRequestAssetBundle (string path)
        {
            if (assetBundleManifest == null)
                return false;
            
            ResourceItem resourceItem = null;

            foreach (var dependency in assetBundleManifest.GetAllDependencies (path))
            {
                if (_dictLoader.ContainsKey (dependency) == true)
                    continue;
                
                if (_dictResourceAssetBundle.TryGetValue (dependency, out resourceItem) == true)
                    resourceItem.IncRef ();
                else
                {
                    BaseLoader loader = BaseLoader.Init<AssetBundleLoader> (dependency);
                    _dictLoader.Add (dependency, loader);
                }
            }

            if (_dictLoader.ContainsKey (path) == false)
            {
                if (_dictResourceAssetBundle.TryGetValue (path, out resourceItem) == false)
                    resourceItem.IncRef ();
                else
                {
                    BaseLoader loader = BaseLoader.Init<AssetBundleLoader> (path);
                    _dictLoader.Add (path, loader);
                }
            }

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

            // TODO

            _dictRequestSet.Remove (label);
            return true;
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

        public Object GetAsset (string url)
        {
            ResourceItem resource = null;
            _dictResourceAsset.TryGetValue (url, out resource);
            return resource.resource;
        }

        public Object GetAssetBundle (string path)
        {
            ResourceItem resource = null;
            _dictResourceAssetBundle.TryGetValue (path, out resource);
            return resource.resource;
        }

        public Texture2D GetTexture (string url)
        {
            ResourceItem resource = null;
            _dictResourceTexture.TryGetValue (url, out resource);
            return resource.resource as Texture2D;
        }
    }
}