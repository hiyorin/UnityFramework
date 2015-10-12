using UnityEngine;
using System.Collections.Generic;

namespace Framework.Scene
{
    public class SceneBase : MonoBehaviour
    {
    	public virtual bool OnSceneCreate () { return true; }
    	public virtual bool OnSceneInitialize () { return true; }
    	public virtual void OnSceneStart () {}
        public virtual void OnSceneVisible () {}
    	public virtual void OnSceneUpdate () {}
        public virtual void OnSceneInvisible () {}
    	public virtual void OnSceneStop () {}
    	public virtual void OnSceneDestroy () {}

    	public virtual void OnSceneMessage (Scene owner, Dictionary<string, object> param) {}

    	protected void SendSceneMessage (Dictionary<string, object> param)
    	{
            SceneManager.Instance.SendMessage (this, param);
    	}

    	protected T GetSubScene<T> () where T:SceneBase
    	{
    		return SceneManager.Instance.GetSubScene<T> (this);
    	}

        protected void Next (string sceneName, FadeType fadeType, bool isRegisterStack)
        {
            SceneManager.Instance.NextScene (sceneName, fadeType, isRegisterStack);
        }

        protected void Next (Scene scene, FadeType fadeType, bool isRegisterStack)
        {
            SceneManager.Instance.NextScene (scene, fadeType, isRegisterStack);
        }

        protected void ChangeSubSceneSlide (Scene beforeScene, Scene afterScene, SlideType slideType)
        {
            SceneManager.Instance.ChangeSubSceneSlide (this, beforeScene, afterScene, slideType);
        }

        protected void ChangeSubSceneSlide (string beforeSceneName, string afterSceneName, SlideType slideType)
        {
            SceneManager.Instance.ChangeSubSceneSlide (this, beforeSceneName, afterSceneName, slideType);
        }
    }
}