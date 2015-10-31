using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Framework.UI;
using DG.Tweening;

namespace Framework.Scene.Resident
{
    [RequireComponent (typeof (UICanvasPanel))]
    public class CommonDialogController : MonoBehaviour
    {
        public class DialogEvent : UnityEvent<CommonDialogController> {}

        public readonly DialogEvent onOpen = new DialogEvent ();
        public readonly DialogEvent onClose = new DialogEvent ();
        public readonly DialogEvent onClickOneButton = new DialogEvent ();
        public readonly DialogEvent onClickTwoButton = new DialogEvent ();
        public readonly DialogEvent onClickTreeButton = new DialogEvent ();

        [SerializeField]
        private float _fadeDuration = 0.25f;
        [SerializeField]
        private Text _message = null;
        [SerializeField]
        private Button _oneButton = null;
        [SerializeField]
        private Text _oneButtonText = null;
        [SerializeField]
        private Button _twoButton = null;
        [SerializeField]
        private Text _twoButtonText = null;
        [SerializeField]
        private Button _treeButton = null;
        [SerializeField]
        private Text _treeButtonText = null;

        protected UICanvasPanel _panel = null;

        private void Awake ()
        {
            _panel = GetComponent<UICanvasPanel> ();
        }

        private void Start ()
        {
            if (_oneButton != null)
                _oneButton.onClick.AddListener (OnClickOneButton);

            if (_twoButton != null)
                _twoButton.onClick.AddListener (OnClickTwoButton);

            if (_treeButton != null)
                _treeButton.onClick.AddListener (OnClickTwoButton);
        }

        public void Open ()
        {
            gameObject.SetActiveSafe (true);
            _panel.FadeIn (_fadeDuration, OnOpenComplete);
        }

        public void Close ()
        {
            _panel.FadeOut (_fadeDuration, OnCloseComplete);
        }

        public void SetText (string message,
            string oneButtonText=null, string twoButtonText=null, string treeButtonText=null)
        {
            _message.text = message;
            if (_oneButtonText != null)
                _oneButtonText.text = oneButtonText;
            if (_twoButtonText != null)
                _twoButtonText.text = twoButtonText;
            if (_treeButtonText != null)
                _treeButtonText.text = treeButtonText;
        }

        private void OnOpenComplete ()
        {
            onOpen.Invoke (this);
        }

        private void OnCloseComplete ()
        {
            gameObject.SetActiveSafe (false);
            onClose.Invoke (this);
        }

        private void OnClickOneButton ()
        {
            onClickOneButton.Invoke (this);
            Close ();
        }

        private void OnClickTwoButton ()
        {
            onClickTwoButton.Invoke (this);
            Close ();
        }

        private void OnClickTreeButton ()
        {
            onClickTreeButton.Invoke (this);
            Close ();
        }
    }
}