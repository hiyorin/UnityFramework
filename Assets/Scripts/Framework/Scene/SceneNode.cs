using UnityEngine;

namespace Framework.Scene
{
    public enum Scene
    {
        Empty,

        // Example
        ExampleSceneManager_A,
        ExampleSceneManager_B,
        ExampleSceneManager_C,
    }

    public enum SceneType
    {
        Standard,
        Static,
        Resident,
    }

    public enum SceneState
    {
    	Empty,
    	Create,
    	Initialize,
    	Start,
    	Visible,
    	Update,
    	Invisible,
    	Stop,
    	Sleep,
    	Destroy,
    }

    public class SceneNode
    {
    	public readonly string name;
    	public readonly GameObject rootObject;
        public readonly SceneType sceneType;

        public SceneState state { set; get; }
        public bool isInitialized { set; get; }
        public bool isVisibled { set; get; }

        private SceneBase _scene = null;
    	public SceneBase scene {
            get {
                if (_scene == null)
                    _scene = rootObject.transform.GetComponentInChildren<SceneBase> ();
                return _scene;
            }
        }

        private SceneProperty _property = null;
    	public SceneProperty property {
            get {
                if (_property == null)
                    _property = rootObject.transform.GetComponentInChildren<SceneProperty> ();
                return _property;
            }
        }

        public SceneNode (string name, GameObject rootObject) : this (name, rootObject, SceneType.Standard)
    	{
    	}

        public SceneNode (string name, GameObject rootObject, SceneType sceneType)
    	{
    		this.name = name;
            this.rootObject = rootObject;
            this.sceneType = sceneType;
            this.state = SceneState.Empty;
            this.isInitialized = false;
            this.isVisibled = true;

            this._scene = scene;
            this._property = property;
    	}

    	public bool isActive {
    		get { return rootObject.activeSelf; }
    		set { rootObject.SetActive (value); }
    	}
    }
}
