using System.Collections;
#if !UNITY_EDITOR
using UnityEngine;
#else
using UnityEditor;
#endif
namespace Framework.Resource.Loader
{
    public enum LoaderState
    {
        Wait,
        Connecting,
        Successed,
        Failed,
    }

    public abstract class BaseLoader
    {
        public static T Init<T> (string path) where T:BaseLoader, new()
        {
            T loader = new T ();
            loader.Load (path);
            return loader;
        }

        private IEnumerator _iterator = null;

        public string path { private set; get; }
        public LoaderState state { private set; get; }
        public string error { private set; get; }

        protected BaseLoader ()
        {
            this.state = LoaderState.Wait;
        }

        public bool isComplete {
            get { return state == LoaderState.Successed || state == LoaderState.Failed; }
        }

        public bool isSuccessed {
            get { return state == LoaderState.Successed; }
        }

        public bool isFailed {
            get { return state == LoaderState.Failed; }
        }

        protected abstract IEnumerator GenerateLoadProcess (string path);

        public bool MoveNext ()
        {
            if (_iterator == null)
                return false;

            return _iterator.MoveNext ();
        }

        private bool Load (string path)
        {
            if (_iterator != null)
                return false;
            
            this.path = path;
            this.state = LoaderState.Connecting;
            _iterator = GenerateLoadProcess (path);

            return true;
        }

        protected void Successed ()
        {
            state = LoaderState.Successed;
        }

        protected void Failed (string text)
        {
            state = LoaderState.Failed;
            error = text;
        }

#if UNITY_EDITOR
        protected static string GetPlatformFolderForAssetBundles ()
        {
            switch(EditorUserBuildSettings.activeBuildTarget)
            {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.WebPlayer:
                return "WebPlayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return null;
            }
        }
#else
        protected static string GetPlatformFolderForAssetBundles ()
        {
            switch (Application.platform)
            {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.OSXWebPlayer:
                return "WebPlayer";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
                // Add more build platform for your own.
                // If you add more platforms, don't forget to add the same targets to GetPlatformFolderForAssetBundles(BuildTarget) function.
            default:
                return null;
            }
        }
#endif
    }
}