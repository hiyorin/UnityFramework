using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.Scene;
using Framework.Resource;
using Framework.Resource.Loader;
using DG.Tweening;

public class DownloadSceneManager : SingletonSceneBase<DownloadSceneManager>
{
    [Header ("Components")]
    [SerializeField]
    private CanvasGroup _canvasGroup = null;
    [SerializeField]
    private Image _imgBackground = null;
    [SerializeField]
    private Slider _slider = null;
    [SerializeField]
    private Text _textValue = null;
    [SerializeField]
    private Text _textMax = null;

    [Header ("Parameters")]
    [SerializeField]
    private float _fadeDuration = 0.5f;

    private readonly List<AssetBundleDependencyLoader> _listLoader = new List<AssetBundleDependencyLoader> ();

    public bool isVisible { private set; get; }

    public override bool OnSceneCreate ()
    {
        _canvasGroup.alpha = 0.0f;
        return true;
    }

    public override void OnSceneUpdate ()
    {
        if (_listLoader.Count <= 0)
            return;

        if (ResourceManager.Instance.IsAssetBundleManifestExists () == false)
            return;
        if (ResourceManager.Instance.IsAssetBundleCRCExists () == false)
            return;

        if (isVisible == false)
        {
            int index = 0;
            while (_listLoader.Count < index)
            {
                var loader = _listLoader [index];
                Hash128 hash = ResourceManager.Instance.assetBundleManifest.GetAssetBundleHash (loader.path);
                if (Caching.IsVersionCached (loader.GetURL (), hash) == true) {
                    _listLoader.Remove (loader);
                }
                else
                {
                    index++;
                }
            }

            if (_listLoader.Count > 0)
            {
                Show ();
            }
        }

        int completeCount = 0;
        float progress = 0.0f;
        foreach (var loader in _listLoader)
        {
            if (loader.isComplete == true)
            {
                completeCount ++;
                progress += 1.0f;
            }
            else
            {
                progress += loader.progress;
            }
        }

        _slider.value = progress / (float)_listLoader.Count;
        _textValue.text = completeCount.ToString ();
        _textMax.text = _listLoader.Count.ToString ();

        if (completeCount >= _listLoader.Count)
        {
            _listLoader.Clear ();
            Hide ();
        }
    }

    public bool Add (AssetBundleDependencyLoader loader)
    {
        Debug.Log ("Add");
        if (loader == null)
            return false;
        if (LoaderPathContains (loader.path) == true)
            return false;

        if (_listLoader.Count <= 0)
        {
            Initialize ();
        }

        _listLoader.Add (loader);

        return true;
    }

    private void Initialize ()
    {
        Debug.Log ("Initialize");
        ResourceManager.Instance.LoadAssetBundleManifest ();
        ResourceManager.Instance.LoadAssetBundleCRC ();
        
        _slider.value = 0;
        _textValue.text = "0";
        _textMax.text = _listLoader.Count.ToString ();
    }

    private void Show ()
    {
        isVisible = true;
        _canvasGroup.DOFade (1.0f, _fadeDuration)
            .OnPlay (OnFadeInPlay)
            .OnComplete (OnFadeInComplete)
            .Play ();
    }

    private void Hide ()
    {
        isVisible = false;
        _canvasGroup.DOFade (0.0f, _fadeDuration)
            .OnPlay (OnFadeOutPlay)
            .OnComplete (OnFadeOutComplete)
            .Play ();
    }

    private bool LoaderPathContains (string path)
    {
        foreach (var loader in _listLoader)
            if (loader.path == path)
                return true;
        return false;
    }

    void OnFadeInPlay ()
    {
        _imgBackground.enabled = true;
    }

    void OnFadeInComplete ()
    {
    }

    void OnFadeOutPlay ()
    {
    }

    void OnFadeOutComplete ()
    {
        _imgBackground.enabled = false;

    }
}
