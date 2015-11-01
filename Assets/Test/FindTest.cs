using UnityEngine;

public class FindTest : MonoBehaviour
{
    void Start ()
    {
        Debug.Log ("GameObject Child_1 " + GameObject.Find ("Child_1"));
        Debug.Log ("GameObject Child_2 " + GameObject.Find ("Child_2"));
        Debug.Log ("GameObject Child_3 " + GameObject.Find ("Child_3"));
        Debug.Log ("");
        Debug.Log ("Transform Find Child_1 " + transform.Find ("Child_1"));
        Debug.Log ("Transform Find Child_2 " + transform.Find ("Child_2"));
        Debug.Log ("Transform Find Child_3 " + transform.Find ("Child_3"));
        Debug.Log ("");
        Debug.Log ("Transform FindChild Child_1 " + transform.FindChild ("Child_1"));
        Debug.Log ("Transform FindChild Child_2 " + transform.FindChild ("Child_2"));
        Debug.Log ("Transform FindChild Child_3 " + transform.FindChild ("Child_3"));
        Debug.Log ("");
        Debug.Log ("Transform FindChildAll Child_1 " + transform.FindChildAll ("Child_1"));
        Debug.Log ("Transform FindChildAll Child_2 " + transform.FindChildAll ("Child_2"));
        Debug.Log ("Transform FindChildAll Child_3 " + transform.FindChildAll ("Child_3"));
    }
}
