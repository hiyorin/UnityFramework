using UnityEngine;
using DG.Tweening;

namespace Framework.UI
{
    [RequireComponent (typeof (CanvasGroup))]
    public class UICanvasPanel : UIMonoBehaviour
    {
        private Vector3 _defaultLocalPosition = Vector3.zero;
        private bool _isTransitionComplete = true;

        private CanvasGroup _canvasGroup;
        protected CanvasGroup canvasGroup {
            get {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup> ();
                return _canvasGroup;
            }
        }

        public bool isTransitionComplete {
            protected set { _isTransitionComplete = value; }
            get { return _isTransitionComplete; }
        }

        protected override void Reset ()
        {
            base.Reset ();
            OnValidate ();
        }

        protected override void OnValidate ()
        {
            base.OnValidate ();
            rectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
            rectTransform.pivot = new Vector2 (0.5f, 0.5f);
            rectTransform.sizeDelta = rootUICanvas.rectTransform.sizeDelta;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        protected override void Awake ()
        {
            base.Awake ();
            _defaultLocalPosition = rectTransform.localPosition;
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

        public virtual void SlideIn (float duration, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;
            
            isTransitionComplete = false;
            rectTransform.localPosition = _defaultLocalPosition + new Vector3 (rectTransform.sizeDelta.x, 0.0f);
            Tweener tween = rectTransform.DOLocalMoveX (0.0f, duration).OnComplete (OnCompleteTransition);
            if (onComplete != null)
                tween.OnComplete (onComplete);
            tween.Play ();
        }

        public virtual void SlideOut (float duration, TweenCallback onComplete=null)
        {
            if (isTransitionComplete == false)
                return;

            isTransitionComplete = false;
            rectTransform.localPosition = _defaultLocalPosition;
            Tweener tween = rectTransform.DOLocalMoveX (_defaultLocalPosition.x - rectTransform.sizeDelta.x, duration).OnComplete (OnCompleteTransition);
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