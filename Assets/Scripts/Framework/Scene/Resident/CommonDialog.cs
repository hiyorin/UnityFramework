using UnityEngine;
using Framework.UI;

public class CommonDialog : UIMonoBehaviour
{
    [SerializeField]
    private CommonDialogController _announce = null;
    [SerializeField]
    private CommonDialogController _oneButton = null;
    [SerializeField]
    private CommonDialogController _twoButton = null;
    [SerializeField]
    private CommonDialogController _treeButton = null;

    public CommonDialogController announce {
        get { return _announce; }
    }

    public CommonDialogController oneButton {
        get { return _oneButton; }
    }

    public CommonDialogController twoButton {
        get { return _twoButton; }
    }

    public CommonDialogController treeButton {
        get { return _treeButton; }
    }

    public void Initialize ()
    {
        RemoveListener (_announce);
        RemoveListener (_oneButton);
        RemoveListener (_twoButton);
        RemoveListener (_treeButton);
    }

    public bool ContainsController (CommonDialogController controller)
    {
        if (announce.Equals (controller) == true)
            return true;
        if (oneButton.Equals (controller) == true)
            return true;
        if (twoButton.Equals (controller) == true)
            return true;
        if (treeButton.Equals (controller) == true)
            return true;
        return false;
    }

    private void RemoveListener (CommonDialogController controller)
    {
        if (controller == null)
            return;

        controller.onOpen.RemoveAllListeners ();
        controller.onClose.RemoveAllListeners ();
        controller.onClickOneButton.RemoveAllListeners ();
        controller.onClickTwoButton.RemoveAllListeners ();
        controller.onClickTreeButton.RemoveAllListeners ();

        controller.gameObject.SetActiveSafe (false);
    }
}