using UnityEngine;

namespace Framework.Scene
{
    public abstract class SingletonSceneBase<T> : SceneBase where T : SingletonSceneBase<T>
    {
        private static T instance = null;

        public static T Instance {
            get { return GetInstance (); }
        }

        public static T GetInstance ()
        {
            if (instance == null)
                Debug.LogWarning (string.Format ("SingletonSceneBase {0} is null.", typeof(T).Name));
            
            return instance;
        }

        private void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance.Equals (this) == false)
            {
                Destroy (this);
            }
        }
    }
}