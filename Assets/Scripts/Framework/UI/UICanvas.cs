using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace Framework.UI
{
	[RequireComponent (typeof (Canvas), typeof (CanvasScaler), typeof (CanvasGroup))]
	[RequireComponent (typeof (GraphicRaycaster))]
    public class UICanvas : UICanvasPanel
	{
		[SerializableAttribute]
		public enum Type
		{
			UIBackground,
			UI,
			UIPopup,
            Transition,
		}

		[SerializeField]
		private Type m_type = Type.UI;

		private Canvas m_canvas;
		protected Canvas canvas {
			get {
				if (m_canvas == null)
					m_canvas = GetComponent<Canvas> ();
				return m_canvas;
			}
		}

        private CanvasScaler m_canvasScaler;
        protected CanvasScaler canvasScaler {
            get {
                if (m_canvasScaler == null)
                    m_canvasScaler = GetComponent<CanvasScaler> ();
                return m_canvasScaler;
            }
        }

        public bool isRootCanvas {
            get { return canvas.isRootCanvas; }
        }

        protected override void Reset ()
        {
            base.Reset ();
            gameObject.layer = LayerMask.NameToLayer ("UI");
        }

		protected override void OnValidate ()
		{
			base.OnValidate ();
            name = typeof (UICanvas).Name;

			switch (m_type)
			{
			case Type.UIBackground:
				canvas.sortingOrder = -1000;
				break;
			case Type.UI:
				canvas.sortingOrder = 0;
				break;
			case Type.UIPopup:
				canvas.sortingOrder = 1000;
				break;
            case Type.Transition:
                canvas.sortingOrder = 2000;
                break;
			default:
				Debug.LogError (m_type);
				break;
			}

            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.referenceResolution =
                new Vector2 (SceneManagerSettings.ScreenWidth, SceneManagerSettings.ScreenHeight);
            canvasScaler.matchWidthOrHeight = SceneManagerSettings.ScreenMatchWidthOrHeight;
		}

		protected override void Start ()
		{
			base.Start ();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			switch (m_type)
			{
			case Type.UIBackground:
				canvas.worldCamera = CameraManager.Instance.uiBackgroundCamera;
				break;
			case Type.UI:
			case Type.UIPopup:
            case Type.Transition:
				canvas.worldCamera = CameraManager.Instance.uiCamera;
				break;
			default:
				Debug.LogError (m_type);
				break;
			}
		}

        public override void SlideIn (float duration, SlideTo to, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;
            
            foreach (RectTransform trans in rectTransform)
            {
                UICanvasPanel panel = trans.GetComponent<UICanvasPanel> ();
                if (panel != null)
                    panel.SlideIn (duration, to, onComplete);
            }
        }

        public override void SlideOut (float duration, SlideTo to, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;
            
            foreach (RectTransform trans in rectTransform)
            {
                UICanvasPanel panel = trans.GetComponent<UICanvasPanel> ();
                if (panel != null)
                    panel.SlideOut (duration, to, onComplete);
            }
        }

        public override bool isTransitionComplete {
            get {
                foreach (RectTransform trans in rectTransform)
                {
                    UICanvasPanel panel = trans.GetComponent<UICanvasPanel> ();
                    if (panel != null && panel.isTransitionComplete == false)
                        return false;
                }
                return true;
            }
        }
	}
}
