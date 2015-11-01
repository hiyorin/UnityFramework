using UnityEngine;

public class FindTest : MonoBehaviour
{
    void Start ()
    {
        Debug.Log ("Find Child_1 " + transform.Find ("Child_1"));
        Debug.Log ("Find Child_2 " + transform.Find ("Child_2"));
        Debug.Log ("Find Child_3 " + transform.Find ("Child_3"));

        Debug.Log ("FindChild Child_1 " + transform.FindChild ("Child_1"));
        Debug.Log ("FindChild Child_2 " + transform.FindChild ("Child_2"));
        Debug.Log ("FindChild Child_3 " + transform.FindChild ("Child_3"));
    }
}
