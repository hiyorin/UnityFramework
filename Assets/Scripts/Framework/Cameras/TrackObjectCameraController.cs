using UnityEngine;

namespace Framework.Cameras
{
    [ExecuteInEditMode()]
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

            switch (lookAtType)
            {
            case LookAtType.Target:
                LookAtTarget ();
                break;
            case LookAtType.BasePosition:
                LookAtBasePosition ();
                break;
            default:
                Debug.LogError (lookAtType);
                break;
            }
        }

        private void LookAtTarget ()
        {
            Vector3 forword = target.position - targetCamera.transform.position + lookAtOffset;
            if (forword == Vector3.zero)
                return;

            Quaternion dir = Quaternion.LookRotation (forword);
            float rotationStep = rotationSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.RotateTowards (targetCamera.transform.rotation, dir, rotationStep);
            targetCamera.transform.rotation = rotation;
        }

        private void LookAtBasePosition ()
        {
            Vector3 forword = (basePosition + lookAtOffset) - targetCamera.transform.position;
            if (forword == Vector3.zero)
                return;
            
            Quaternion dir = Quaternion.LookRotation (forword);
            float rotationStep = rotationSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.RotateTowards (targetCamera.transform.rotation, dir, rotationStep);
            targetCamera.transform.rotation = rotation;
        }

        #if UNITY_EDITOR
        protected override void OnEditorUpdateCamera ()
        {
            float moveStep = moveSpeed * Time.deltaTime;
            basePosition = Vector3.MoveTowards (basePosition, target.transform.position, moveStep);
            Vector3 position = basePosition + positionOffset;
            targetCamera.transform.position = position;

            switch (lookAtType)
            {
            case LookAtType.Target:
                targetCamera.transform.LookAt (target.position + lookAtOffset);
                break;
            case LookAtType.BasePosition:
                targetCamera.transform.LookAt (basePosition + lookAtOffset);
                break;
            default:
                Debug.LogError (lookAtType);
                break;
            }
        }
        #endif
    }
}