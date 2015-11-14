using UnityEngine;
using System.Collections;

namespace Framework.Cameras
{
    public class GestureCameraController : CameraController
    {
        public float distance = 10.0f;

        private Vector3 deltaMove = Vector3.zero;

        protected override void OnUpdateCamera ()
        {
            targetCamera.transform.position += deltaMove;
            deltaMove = Vector3.zero;
        }

        public void Drag (Vector3 screenDeltaMove)
        {
            if (screenDeltaMove == Vector3.zero)
                return;
            
            if (isActiveTargetOwners == true)
                return;
            
            Vector3 baseWorldPos = targetCamera.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, distance));
            Vector3 deltaWorldPos = targetCamera.ScreenToWorldPoint (new Vector3 (1.0f, 0.0f, distance));
            float completion = deltaWorldPos.x - baseWorldPos.x;

            Vector3 delta = screenDeltaMove.SwapYZ () * completion;
            Vector3 angle = new Vector3 (0.0f, targetCamera.transform.rotation.eulerAngles.y, 0.0f);
            delta = Quaternion.Euler (angle) * delta;
            deltaMove = delta;
        }
    }
}