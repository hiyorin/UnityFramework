using UnityEngine;
using Framework.Scene;
using Framework.Cameras;

public class ExampleCameraController : SceneBase
{
    [SerializeField]
    private GestureCameraController gestureCamera = null;

    private bool isPress = false;
    private Vector3 touchPosition = Vector3.zero;

    public override void OnSceneUpdate ()
    {
        if (Input.GetMouseButton (0) == true)
        {
            if (isPress == true)
            {
                Vector3 delta = touchPosition - Input.mousePosition;
                gestureCamera.Drag (delta);
            }
            else
            {
                isPress = true;
            }
            touchPosition = Input.mousePosition;
        }
        else
        {
            isPress = false;
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        gestureCamera.Pinch (scrollDelta);
    } 
}
