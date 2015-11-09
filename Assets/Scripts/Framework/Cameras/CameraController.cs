using UnityEngine;
using UnityEngine.Events;

namespace Framework.Cameras
{
    public class CameraController : MonoBehaviour
    {
        public class CameraEvent : UnityEvent<CameraController> {}

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

        void Update ()
        {
            if (updateType != UpdateType.Update)
                return;

            if (targetCamera == null || target == null)
                return;

            OnUpdateCamera ();
        }

        void LateUpdate ()
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
            if (targetCamera == null || target == null)
                return;
            
            OnEditorUpdateCamera ();
        }

        protected virtual void OnEditorUpdateCamera ()
        {
            OnUpdateCamera ();
        }
        #endif
    }
}