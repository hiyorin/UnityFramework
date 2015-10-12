using UnityEngine;
using System.Collections.Generic;

namespace Framework.Scene
{
    [System.Serializable]
    public class SubSceneProperty
    {
        public readonly string sceneName;
        public bool isVisible;
#if UNITY_EDITOR
        public readonly string sceneFullName;

        public SubSceneProperty (string sceneName, bool isVisible, string sceneFullName)
        {
            this.sceneName = sceneName;
            this.isVisible = isVisible;
            this.sceneFullName = sceneFullName;
        }
#endif
        public SubSceneProperty (string sceneName, bool isVisible)
        {
            this.sceneName = sceneName;
            this.isVisible = isVisible;
        }
    }

    public class SceneProperty : MonoBehaviour
    {
        [SerializeField]
        private List<SubSceneProperty> _listAddSubScene = new List<SubSceneProperty> ();
        [SerializeField]
        private bool _isAlwaysDestroy = false;

    	public bool isAlwaysDestroy {
#if UNITY_EDITOR
    		set { _isAlwaysDestroy = value; }
#endif
    		get { return _isAlwaysDestroy; }
    	}

        public List<SubSceneProperty> listAddSubScene {
#if UNITY_EDITOR
            set { _listAddSubScene = value; }
#endif
            get { return _listAddSubScene; }
        }
    }
}