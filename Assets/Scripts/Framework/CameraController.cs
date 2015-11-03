using UnityEngine;

namespace Framework
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera _targetCamera = null;
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        private Vector3 _positionOffset = Vector3.zero;
        [SerializeField]
        private Vector3 _lookAtOffset = Vector3.zero;

        public Camera targetCamera {
            get { return _targetCamera; }
            set { _targetCamera = value; }
        }

        public Transform target {
            get { return _target; }
            set { _target = value; }
        }

        public Vector3 positionOffset {
            get { return _positionOffset; }
            set { _positionOffset = value; }
        }

        public Vector3 lookAtOffset {
            get { return _lookAtOffset; }
            set { _lookAtOffset = value; }
        }
                
        private void Update ()
        {
            if (_targetCamera == null || _target == null)
                return;

            Vector3 position = _target.transform.position + _positionOffset;
            _targetCamera.transform.position = position;

            Vector3 lookAtPosition = _target.position + _lookAtOffset;
            _targetCamera.transform.LookAt (lookAtPosition);
        }
    }
}