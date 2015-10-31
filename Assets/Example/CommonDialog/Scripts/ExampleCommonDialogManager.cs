using UnityEngine;
using UnityEngine.UI;
using Framework.Scene;

public class ExampleCommonDialogManager : SingletonSceneBase<ExampleCommonDialogManager>
{
    [SerializeField]
    private Text _message = null;

    public void OnClickAnnounce ()
    {
        CommonDialogController dialog = CommonDialogSceneManager.Instance.CreateAnnounce ();
        dialog.onOpen.AddListener (
            (CommonDialogController arg0) => {
                Log ("Announce Open");
            }
        );
        dialog.onClose.AddListener (
            (CommonDialogController arg0) => {
                Log ("Announce Close");
            }
        );
        dialog.SetText ("Announce");
        dialog.Open ();
    }

    public void OnClickOneButton ()
    {
        CommonDialogController dialog = CommonDialogSceneManager.Instance.CreateOneButton ();
        dialog.onClickOneButton.AddListener (
            (CommonDialogController arg0) => {
                Log ("OneButton 1");
            }
        );
        dialog.SetText ("OneButton", "OK");
        dialog.Open ();
    }

    public void OnClickTwoButton ()
    {
        CommonDialogController dialog = CommonDialogSceneManager.Instance.CreateTwoButton ();
        dialog.onClickOneButton.AddListener (
            (CommonDialogController arg0) => {
                Log ("TwoButton 1");
            }
        );
        dialog.onClickOneButton.AddListener (
            (CommonDialogController arg0) => {
                Log ("TwoButton 2");
            }
        );
        dialog.SetText ("TwoButton", "YES", "NO");
        dialog.Open ();
    }

    public void OnClickTreeButton ()
    {
        CommonDialogController dialog = CommonDialogSceneManager.Instance.CreateTreeButton ();
        dialog.onClickOneButton.AddListener (
            (CommonDialogController arg0) => {
                Log ("TreeButton 1");
            }
        );
        dialog.onClickTwoButton.AddListener (
            (CommonDialogController arg0) => {
                Log ("TreeButton 2");
            }
        );
        dialog.onClickTreeButton.AddListener (
            (CommonDialogController arg0) => {
                Log ("TreeButton 3");
            }
        );
        dialog.SetText ("TreeButton", "YES", "NO", "WAIT");
        dialog.Open ();
    }

    private void Log (string text)
    {
        _message.text = text;
        Debug.Log (text);
    }
}