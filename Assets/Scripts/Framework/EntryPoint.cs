using UnityEngine;
using Framework;
using Framework.Scene;
using Framework.Resource;
using Framework.Cameras;
using DG.Tweening;

public sealed class EntryPoint
{
	[RuntimeInitializeOnLoadMethod]
	private static void CreateStaticGameObject ()
    {
//        Object fingerGestures = Object.Instantiate (Resources.Load ("FingerGestures"));
//        SceneManager.Instance.AddIgnoreCollection (fingerGestures.name);

        SceneManager.GetInstance ();
        ResourceManager.GetInstance ();
        CameraManager.GetInstance ();

        SceneManager.Instance.AddIgnoreCollection ("[DOTween]");
        DOTween.Init (true, false, LogBehaviour.ErrorsOnly);
    }
}
