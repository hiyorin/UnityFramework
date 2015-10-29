using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetActiveSafe (this GameObject go, bool isActive)
    {
        if (go.activeSelf != isActive)
        {
            go.SetActive (isActive);
        }
    }
}
