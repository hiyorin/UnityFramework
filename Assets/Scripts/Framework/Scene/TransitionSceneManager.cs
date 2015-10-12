using UnityEngine;
using UnityEngine.UI;
using Framework.UI;
using Framework.Scene;
using DG.Tweening;

public enum FadeType
{
    Black,
    White,
}

public enum SlideType
{
    ToUp,
    ToDown,
    ToLeft,
    ToRight,
}

public class TransitionSceneManager : SingletonSceneBase<TransitionSceneManager>
{
    [SerializeField]
    private float _duration = 0.5f;
    [SerializeField]
    private Image _imgMask = null;
    [SerializeField]
    private Image _imgDefaultFade = null;

    [SerializeField, ReadOnlyAttribute]
    private FadeType _fadeType = FadeType.Black;

    public bool isComplete { private set; get; }

    public float duration {
        set { _duration = value; }
        get { return _duration; }
    }

    public override bool OnSceneCreate ()
    {
        isComplete = true;
        return true;
    }

    public void FadeOut (FadeType fadeType, float duration=float.MinValue)
    {
        if (isComplete == false)
            return;
        
        isComplete = false;
        _fadeType = fadeType;
        _imgMask.enabled = true;
        _imgDefaultFade.enabled = true;

        if (duration < 0.0f)
            duration = _duration;

        switch (_fadeType) {
        case FadeType.Black:
            _imgDefaultFade.DOColor (Color.black, duration).OnComplete (OnCompleteFadeOut).Play ();
            break;
        case FadeType.White:
            _imgDefaultFade.DOColor (Color.white, duration).OnComplete (OnCompleteFadeOut).Play ();
            break;
        default:
            break;
        }
    }

    public void FadeIn ()
    {
        if (isComplete == false)
            return;
        
        isComplete = false;

        switch (_fadeType) {
        case FadeType.Black:
        case FadeType.White:
            _imgDefaultFade.DOColor (Color.clear, _duration).OnComplete (OnCompleteFadeIn).Play ();
            break;
        default:
            break;
        }
    }

    public void Slide (GameObject beforeObject, GameObject afterObject, SlideType slideType)
    {
        if (isComplete == false)
            return;
        
        isComplete = false;
        _imgDefaultFade.enabled = true;

        if (beforeObject != null)
        {
            UICanvas uiCanvas = beforeObject.GetComponentInChildren<UICanvas> ();
            if (uiCanvas != null)
                uiCanvas.SlideOut (_duration, SlideTypeConvert (slideType), OnCompleteSlide);
        }

        if (afterObject != null)
        {
            UICanvas uiCanvas = afterObject.GetComponentInChildren<UICanvas> ();
            if (uiCanvas != null)
                uiCanvas.SlideIn (_duration, SlideTypeConvert (slideType), OnCompleteSlide);
        }
    }

    private UICanvas.SlideTo SlideTypeConvert (SlideType slideType)
    {
        switch (slideType)
        {
        case SlideType.ToUp:
            return UICanvas.SlideTo.Up;
        case SlideType.ToDown:
            return UICanvas.SlideTo.Down;
        case SlideType.ToLeft:
            return UICanvas.SlideTo.Left;
        case SlideType.ToRight:
            return UICanvas.SlideTo.Right;
        default:
            Debug.LogErrorFormat ("{0} NotFound {1}", typeof(SlideType).Name, slideType);
            return UICanvas.SlideTo.Left;
        }
    }

    void OnCompleteFadeOut ()
    {
        isComplete = true;
    }

    void OnCompleteFadeIn ()
    {
        isComplete = true;
        _imgMask.enabled = false;
        _imgDefaultFade.enabled = false;
    }

    void OnCompleteSlide ()
    {
        isComplete = true;
        _imgDefaultFade.enabled = false;
    }
}
