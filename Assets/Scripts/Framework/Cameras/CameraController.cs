using UnityEngine;

namespace Framework.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [System.Serializable]
        public enum UpdateType
        {
            Update,
            LateUpdate,
        }

        public Camera targetCamera = null;
        public Transform target = null;

        public Vector3 positionOffset = Vector3.zero;
        public Vector3 lookAtOffset = Vector3.zero;

        public UpdateType updateType = UpdateType.LateUpdate;

        private void Update ()
        {
            if (updateType != UpdateType.Update)
                return;

            if (targetCamera == null || target == null)
                return;

            OnUpdateCamera ();
        }

        private void LateUpdate ()
        {
            if (updateType != UpdateType.LateUpdate)
                return;
            
            if (targetCamera == null || target == null)
                return;

            OnUpdateCamera ();
        }

        protected virtual void OnUpdateCamera ()
        {
            Vector3 position = target.transform.position + positionOffset;
            targetCamera.transform.position = position;

            Vector3 lookAtPosition = target.position + lookAtOffset;
            targetCamera.transform.LookAt (lookAtPosition);
        }

        #if UNITY_EDITOR
        private void OnValidate ()
        {
            Update ();
            LateUpdate ();
        }
        #endif
    }
}