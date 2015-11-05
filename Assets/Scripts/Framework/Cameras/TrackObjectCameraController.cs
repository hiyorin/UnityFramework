using UnityEngine;

namespace Framework.Cameras
{
    public class TrackObjectCameraController : CameraController
    {
        [System.Serializable]
        public enum LookAtType
        {
            Target,
            BasePosition,
        }

        public LookAtType lookAtType = LookAtType.BasePosition;
        public float moveSpeed = 1.0f;
        public float rotationSpeed = 1.0f;

        private Vector3 basePosition = Vector3.zero;

        protected override void OnUpdateCamera ()
        {
            float moveStep = moveSpeed * Time.deltaTime;
            basePosition = Vector3.MoveTowards (basePosition, target.transform.position, moveStep);
            Vector3 position = basePosition + positionOffset;
            targetCamera.transform.position = position;

            if (lookAtType == LookAtType.Target)
            {
                LookAtTarget ();
            }
            else if (lookAtType == LookAtType.BasePosition)
            {
                LookAtBasePosition ();
            }
        }

        private void LookAtTarget ()
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            Quaternion dir = Quaternion.LookRotation (target.position - targetCamera.transform.position + lookAtOffset);
            Quaternion rotation = Quaternion.RotateTowards (targetCamera.transform.rotation, dir, rotationStep);
            targetCamera.transform.rotation = rotation;
        }

        private void LookAtBasePosition ()
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            Quaternion dir = Quaternion.LookRotation (basePosition - targetCamera.transform.position + lookAtOffset);
            Quaternion rotation = Quaternion.RotateTowards (targetCamera.transform.rotation, dir, rotationStep);
            targetCamera.transform.rotation = rotation;
        }
    }
}