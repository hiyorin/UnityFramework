using UnityEngine;
using Framework.Scene;

namespace Framework.Cameras
{
	public class CameraManager : SingletonMonoBehaviour<CameraManager>
	{
		public const int BGDepth	= 1000;
		public const int UIDepth	= 2000;

		public Camera uiBackgroundCamera { get; private set; }
		public Camera uiCamera { get; private set; }

		protected override void OnInitialize ()
        {
            base.OnInitialize ();
            SceneManager.Instance.AddIgnoreCollection (gameObject.name);
			CreateUIBackgroundCamera ();
			CreateUICamera ();
		}

        protected override void OnFinalize ()
        {
            base.OnFinalize ();
            SceneManager.Instance.RemoveIgnoreCollection (gameObject.name);
        }

		private void CreateUIBackgroundCamera ()
		{
			GameObject go = new GameObject ("UIBackgroundCamera");
			go.transform.SetParent (transform);
			uiBackgroundCamera = go.AddComponent<Camera> ();
			uiBackgroundCamera.cullingMask = LayerMask.GetMask ("UI");
			uiBackgroundCamera.clearFlags = CameraClearFlags.Nothing;
			uiBackgroundCamera.depth = BGDepth;
		}

		private void CreateUICamera ()
		{
			GameObject go = new GameObject ("UICamera");
			go.transform.SetParent (transform);
			uiCamera = go.AddComponent<Camera> ();
			uiCamera.cullingMask = LayerMask.GetMask ("UI");
			uiCamera.clearFlags = CameraClearFlags.Depth;
			uiCamera.depth = UIDepth;
		}
	}
}
