using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Resource.Loader
{
    public class AssetBundleLoader : AssetBundleDependencyLoader
    {
        private readonly List<AssetBundleDependencyLoader> _listSubLoader = new List<AssetBundleDependencyLoader> ();

        private AssetBundleDependencyLoader _mainLoader = null;

        public IEnumerable<AssetBundleDependencyLoader> assetDependencies {
            get { return _listSubLoader; }
        }

        public int dependencyCount {
            get { return _listSubLoader.Count; }
        }

        protected override IEnumerator GenerateLoadProcess ()
        {
            while (Caching.ready == false)
                yield return null;
            
            _mainLoader = BaseLoader.Init<AssetBundleDependencyLoader> (path);

            while (_mainLoader.MoveNext () == true)
                yield return null;

            if (_mainLoader.isFailed == true)
            {
                Failed (_mainLoader.error);
                yield break;
            }

            asset = _mainLoader.asset;

            string[] dependencies = ResourceManager.Instance.assetBundleManifest.GetAllDependencies (path);
            foreach (var dependency in dependencies)
            {
                if (ResourceManager.Instance.Contains (dependency, ResourceType.AssetBundle) == false)
                {
                    _listSubLoader.Add (
                        BaseLoader.Init<AssetBundleDependencyLoader> (dependency));
                }
            }

            while (true)
            {
                int completeCount = 0;
                foreach (var loader in _listSubLoader)
                {
                    if (loader.MoveNext () == false)
                        completeCount ++;
                }

                if (_listSubLoader.Count <= completeCount)
                    break;
                
                yield return null;
            }

            StringBuilder errorString = new StringBuilder ();
            foreach (var loader in _listSubLoader)
            {
                if (loader.isFailed == true)
                    errorString.AppendLine (loader.error);
            }

            if (errorString.Length <= 0)
                Successed ();
            else
                Failed (errorString.ToString ());
        }

        public override bool Contains (string path)
        {
            if (this.path.Equals (path) == true)
                return true;

            if (_mainLoader.path == path)
                return true;

            foreach (var loader in _listSubLoader)
            {
                if (loader.path.Equals (path) == true)
                    return true;
            }

            return false;
        }
    }
}