using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.Scene;

namespace Framework.Scene.Resident
{
    public class CommonDialogSceneManager : SingletonSceneBase<CommonDialogSceneManager>
    {
        private readonly Stack<CommonDialog> _stackPool = new Stack<CommonDialog> ();
        private readonly List<CommonDialog> _listDialog = new List<CommonDialog> ();

        [SerializeField]
        private CommonDialog _prefab = null;
        [SerializeField]
        private float _announeTime = 2.0f;

        public override bool OnSceneCreate ()
        {
            if (_prefab == null)
            {
                Debug.LogError ("prefab null");
                return true;
            }

            _prefab.gameObject.SetActiveSafe (false);
            return true;
        }

        public CommonDialogController CreateAnnounce ()
        {
            CommonDialogController controller = GetCommonDialog ().announce;
            controller.onOpen.AddListener (OnOpenAnnounce);
            controller.onClose.AddListener (OnCloseDialog);
            controller.gameObject.SetActiveSafe (true);
            return controller;
        }

        public CommonDialogController CreateOneButton ()
        {
            CommonDialogController controller = GetCommonDialog ().oneButton;
            controller.onClose.AddListener (OnCloseDialog);
            controller.gameObject.SetActiveSafe (true);
            return controller;
        }

        public CommonDialogController CreateTwoButton ()
        {
            CommonDialogController controller = GetCommonDialog ().twoButton;
            controller.onClose.AddListener (OnCloseDialog);
            controller.gameObject.SetActiveSafe (true);
            return controller;
        }

        public CommonDialogController CreateTreeButton ()
        {
            CommonDialogController controller = GetCommonDialog ().treeButton;
            controller.onClose.AddListener (OnCloseDialog);
            controller.gameObject.SetActiveSafe (true);
            return controller;
        }

        private CommonDialog GetCommonDialog ()
        {
            CommonDialog dialog = null;
            if (_stackPool.Count > 0)
            {
                dialog = _stackPool.Pop ();
            }
            else
            {
                GameObject instance = Object.Instantiate (_prefab.gameObject) as GameObject;
                dialog = instance.GetComponent<CommonDialog> ();
                dialog.transform.SetParent (_prefab.transform.parent);
                dialog.transform.Reset ();
                dialog.rectTransform.sizeDelta = Vector2.zero;
            }
            _listDialog.Add (dialog);
            dialog.gameObject.SetActiveSafe (true);
            dialog.rectTransform.SetSiblingIndex (_stackPool.Count + _listDialog.Count);
            return dialog;
        }

        private void OnOpenAnnounce (CommonDialogController controller)
        {
            StartCoroutine (AnnounceAutoClose (controller, _announeTime));
        }

        private void OnCloseDialog (CommonDialogController controller)
        {
            foreach (var dialog in _listDialog)
            {
                if (dialog.ContainsController (controller) == true)
                {
                    dialog.Initialize ();
                    dialog.rectTransform.SetSiblingIndex (0);
                    dialog.gameObject.SetActiveSafe (false);
                    _listDialog.Remove (dialog);
                    _stackPool.Push (dialog);
                    return;
                }
            }
            Debug.LogError ("not found dialog.");
        }

        private IEnumerator AnnounceAutoClose (CommonDialogController controller, float time)
        {
            yield return new WaitForSeconds (time);

            foreach (var dialog in _listDialog)
            {
                if (dialog.ContainsController (controller) == true)
                {
                    controller.Close ();
                    yield break;
                }
            }
        }

        #if UNITY_EDITOR
        [SerializeField]
        private bool _isUnitTest = false;

        void OnGUI ()
        {
            if (_isUnitTest == false)
                return;
            Rect rect = new Rect (0.0f, 0.0f, Screen.width / 2.0f, Screen.height / 10.0f);
            if (GUI.Button (rect, "Announce") == true)
                CreateAnnounce ().Open ();
            rect.y += rect.height;
            if (GUI.Button (rect, "OneButton") == true)
                CreateOneButton ().Open ();
            rect.y += rect.height;
            if (GUI.Button (rect, "TwoButton") == true)
                CreateTwoButton ().Open ();
            rect.y += rect.height;
            if (GUI.Button (rect, "TreeButton") == true)
                CreateTreeButton ().Open ();
        }
        #endif
    }
}