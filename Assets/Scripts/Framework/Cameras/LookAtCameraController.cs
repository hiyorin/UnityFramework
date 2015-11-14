using UnityEngine;
using UnityEngine.Events;

namespace Framework.Cameras
{
    public class LookAtCameraController : CameraController
    {
        public Vector3 positionOffset = Vector3.zero;
        public Vector3 lookAtOffset = Vector3.zero;

        protected override void OnUpdateCamera ()
        {
            Vector3 position = target.transform.position + positionOffset;
            targetCamera.transform.position = position;

            Vector3 lookAtPosition = target.position + lookAtOffset;
            targetCamera.transform.LookAt (lookAtPosition);
        }
    }
}