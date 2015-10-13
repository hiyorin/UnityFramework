using UnityEngine;
using UnityEngine.UI;
using Framework.Scene;
using DG.Tweening;

public enum IndicatorType
{
    Default,
}

public class IndicatorSceneManager : SingletonSceneBase<IndicatorSceneManager>
{
    [SerializeField]
    private Image _imgMask = null;
    [SerializeField]
    private Image _imgDefaultIndicator = null;

    [SerializeField, ReadOnlyAttribute]
    private IndicatorType _indicatorType = IndicatorType.Default;

    private Tween tween = null;

    public bool isShow {
        get { return _imgMask.enabled; }
    }

    public override bool OnSceneCreate ()
    {
        Hide ();
        return true;
    }

    public void Show (IndicatorType indicatorType)
    {
        _indicatorType = indicatorType;
        switch (indicatorType)
        {
        case IndicatorType.Default:
            #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_PSM)
            #if UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle (AndroidActivityIndicatorStyle.Large);
            #elif UNITY_IOS
            Handheld.SetActivityIndicatorStyle (UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
            #endif
            Handheld.StartActivityIndicator ();
            #else
            _imgDefaultIndicator.enabled = true;
            tween = DOTween.Sequence ()
                .Append (_imgDefaultIndicator.transform
                    .DOLocalRotate (new Vector3 (0.0f, 0.0f, -180.0f), 0.5f)
                    .SetEase (Ease.InSine))
                .Append (_imgDefaultIndicator.transform
                    .DOLocalRotate (new Vector3 (0.0f, 0.0f, -360.0f), 0.5f)
                    .SetEase (Ease.OutSine))
                .SetLoops (-1,LoopType.Restart)
                .Play ();
            #endif
            break;
        default:
            Debug.LogErrorFormat ("{0} NotFound {1}", typeof(IndicatorType).Name, indicatorType);
            break;
        }
        _imgMask.enabled = true;
    }

    public void Hide ()
    {
        switch (_indicatorType)
        {
        case IndicatorType.Default:
            #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_PSM)
            Handheld.StopActivityIndicator ();
            #else
            _imgDefaultIndicator.enabled = false;
            #endif
            break;
        default:
            Debug.LogErrorFormat ("{0} NotFound {1}", typeof(IndicatorType).Name, _indicatorType);
            break;
        }
        if (tween != null)
        {
            tween.Kill ();
            tween = null;
        }
        _imgMask.enabled = false;
    }
}
