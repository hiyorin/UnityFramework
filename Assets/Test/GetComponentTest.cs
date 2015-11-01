using UnityEngine;

public class GetComponentTest : MonoBehaviour
{
    void Start ()
    {
        Debug.Log (GetComponent<MeshCollider> ());
        Debug.Log (GetComponentInChildren<BoxCollider> ());
        Debug.Log (GetComponentInChildren<SphereCollider> ());
        Debug.Log (GetComponentInChildren<CapsuleCollider> ());
    }
}
