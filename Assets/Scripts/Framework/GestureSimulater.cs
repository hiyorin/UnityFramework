using UnityEngine;

namespace Framework
{
    public class GestureSimulater : MonoBehaviour
    {
        public Vector3 dragDelta { private set; get; }
        public float pinchDelta { private set; get; }
        public float twistDelta { private set; get; }

        private bool isPress = false;
        private Vector3 touchPosition = Vector3.zero;

    	private void Update ()
        {
            dragDelta = Vector3.zero;
            pinchDelta = 0.0f;
            twistDelta = 0.0f;

            if (Input.GetMouseButton (0) == true)
            {
                if (isPress == true)
                {
                    dragDelta = touchPosition - Input.mousePosition;
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

            pinchDelta = Input.mouseScrollDelta.y;

            twistDelta = Input.mouseScrollDelta.x;
    	}
    }
}