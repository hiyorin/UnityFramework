using UnityEngine;
using Framework.Scene;
using Framework.Resource;
using Framework.Cameras;
using DG.Tweening;

public sealed class EntryPoint
{
	[RuntimeInitializeOnLoadMethod]
	private static void CreateStaticGameObject ()
    {
        SceneManager.GetInstance ();
        ResourceManager.GetInstance ();
        CameraManager.GetInstance ();

        SceneManager.Instance.AddIgnoreCollection ("[DOTween]");
        DOTween.Init (true, false, LogBehaviour.ErrorsOnly);
    }
}
