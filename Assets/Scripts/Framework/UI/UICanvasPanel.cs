using UnityEngine;
using System;
using DG.Tweening;

namespace Framework.UI
{
    [RequireComponent (typeof (CanvasGroup))]
    public class UICanvasPanel : UIMonoBehaviour
    {
        [Serializable]
        public enum Resize
        {
            Start,
            Update,
        }

        public enum SlideTo
        {
            Up,
            Down,
            Left,
            Right,
        }

        [SerializeField]
        private Resize _resize = Resize.Start;

        private Vector3 _defaultLocalPosition = Vector3.zero;

        private CanvasGroup _canvasGroup;
        protected CanvasGroup canvasGroup {
            get {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup> ();
                return _canvasGroup;
            }
        }

        public virtual bool isTransitionComplete {
            private set;
            get;
        }

        protected override void Reset ()
        {
            base.Reset ();
            ResizePanel ();
        }

        protected override void Awake ()
        {
            base.Awake ();
            isTransitionComplete = true;
            _defaultLocalPosition = rectTransform.localPosition;
        }

        protected override void Start ()
        {
            base.Start ();
            if (_resize == Resize.Start)
                ResizePanel ();
        }

        void Update ()
        {
            if (_resize == Resize.Update)
                ResizePanel ();
        }

        protected override void OnDestroy ()
        {
            ResizePanel ();
        }

        private void ResizePanel ()
        {
            rectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
            rectTransform.pivot = new Vector2 (0.5f, 0.5f);
            rectTransform.sizeDelta = rootUICanvas.rectTransform.sizeDelta;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        public virtual void FadeIn (float duration, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;
            
            isTransitionComplete = false;
            Tweener tween = _canvasGroup.DOFade (1.0f, duration).OnComplete (OnCompleteTransition);
            if (onComplete != null)
                tween.OnComplete (onComplete);
            tween.Play ();
        }

        public virtual void FadeOut (float duration, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;

            isTransitionComplete = false;
            Tweener tween = _canvasGroup.DOFade (0.0f, duration).OnComplete (OnCompleteTransition);
            if (onComplete != null)
                tween.OnComplete (onComplete);
            tween.Play ();
        }

        public virtual void SlideIn (float duration, SlideTo to, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;
            isTransitionComplete = false;

            switch (to)
            {
            case SlideTo.Up:
                rectTransform.localPosition = _defaultLocalPosition + new Vector3 (0.0f, -rectTransform.sizeDelta.y);
                break;
            case SlideTo.Down:
                rectTransform.localPosition = _defaultLocalPosition + new Vector3 (0.0f, rectTransform.sizeDelta.y);
                break;
            case SlideTo.Left:
                rectTransform.localPosition = _defaultLocalPosition + new Vector3 (rectTransform.sizeDelta.x, 0.0f);
                break;
            case SlideTo.Right:
                rectTransform.localPosition = _defaultLocalPosition + new Vector3 (-rectTransform.sizeDelta.x, 0.0f);
                break;
            default:
                Debug.LogErrorFormat ("{0} NotFoend {1}", typeof (SlideTo).Name, to);
                break;
            }

            Sequence tween = DOTween.Sequence ().Append (rectTransform
                .DOLocalMoveX (0.0f, duration)
                .OnComplete (OnCompleteTransition));

            if (onComplete != null)
                tween.OnComplete (onComplete);
            tween.Play ();
        }

        public virtual void SlideOut (float duration, SlideTo to, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;
            isTransitionComplete = false;

            rectTransform.localPosition = _defaultLocalPosition;
            Sequence tween = DOTween.Sequence ();
            switch (to)
            {
            case SlideTo.Up:
                tween.Append (rectTransform
                    .DOLocalMoveY (_defaultLocalPosition.y + rectTransform.sizeDelta.y, duration)
                    .OnComplete (OnCompleteTransition));
                break;
            case SlideTo.Down:
                tween.Append (rectTransform
                    .DOLocalMoveY (_defaultLocalPosition.y - rectTransform.sizeDelta.y, duration)
                    .OnComplete (OnCompleteTransition));
                break;
            case SlideTo.Left:
                tween.Append (rectTransform
                    .DOLocalMoveX (_defaultLocalPosition.x - rectTransform.sizeDelta.x, duration)
                    .OnComplete (OnCompleteTransition));
                break;
            case SlideTo.Right:
                tween.Append (rectTransform
                    .DOLocalMoveX (_defaultLocalPosition.x + rectTransform.sizeDelta.x, duration)
                    .OnComplete (OnCompleteTransition));
                break;
            default:
                Debug.LogErrorFormat ("{0} NotFoend {1}", typeof (SlideTo).Name, to);
                break;
            }
            if (onComplete != null)
                tween.OnComplete (onComplete);
            tween.Play ();
        }
        
        protected void OnCompleteTransition ()
        {
            isTransitionComplete = true;
        }
    }
}