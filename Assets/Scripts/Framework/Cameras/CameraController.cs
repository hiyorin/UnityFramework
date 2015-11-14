using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Framework.Cameras
{
    public abstract class CameraController : MonoBehaviour
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
        public UpdateType updateType = UpdateType.LateUpdate;
        public List<CameraController> owners = new List<CameraController> ();

        public virtual bool isActiveTarget {
            get { return (targetCamera != null && target != null); }
        }

        public bool isActiveTargetOwners {
            get {
                if (owners == null)
                    return false;
                foreach (var owner in owners)
                    if (owner.isActiveTarget == true)
                        return true;
                return false;
            }
        }

        private void Update ()
        {
            if (updateType != UpdateType.Update)
                return;

            if (isActiveTarget == false)
                return;

            if (isActiveTargetOwners == true)
                return;

            OnUpdateCamera ();
        }

        private void LateUpdate ()
        {
            if (updateType != UpdateType.LateUpdate)
                return;

            if (isActiveTarget == false)
                return;

            if (isActiveTargetOwners == true)
                return;

            OnUpdateCamera ();
        }

        protected abstract void OnUpdateCamera ();

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