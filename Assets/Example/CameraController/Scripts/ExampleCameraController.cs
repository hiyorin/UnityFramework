using UnityEngine;
using Framework;
using Framework.Scene;
using Framework.Cameras;

public class ExampleCameraController : SceneBase
{
    [SerializeField]
    private GestureCameraController gestureCamera = null;
    [SerializeField]
    private GestureSimulater gestureSimulater = null;

    private bool isPress = false;
    private Vector3 touchPosition = Vector3.zero;

    public override void OnSceneUpdate ()
    {
        gestureCamera.Drag (gestureSimulater.dragDelta);
        gestureCamera.Pinch (gestureSimulater.pinchDelta);
        gestureCamera.Twist (gestureSimulater.twistDelta);

    } 
}
