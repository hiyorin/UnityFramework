using UnityEngine;
using Framework.Scene;
using Framework.Resource;

public class ExampleResourceManager_C : SceneBase
{
    private enum CreateState
    {
        ResourceRequest,
        ResourceRequestWait,
        Complete,
    }

    private readonly string RequestResourceLabel = "ExampleResourceManager_C";

    private CreateState createState = CreateState.ResourceRequest;
    private ResourceRequestSet resourceRequestSet = new ResourceRequestSet ();

    private GameObject instance = null;

    public override bool OnSceneCreate ()
    {
        switch (createState)
        {
        case CreateState.ResourceRequest:
            resourceRequestSet.Add ("cube", ResourceType.AssetBundle);
            ResourceManager.Instance.RegisterRequestSet (
                RequestResourceLabel, resourceRequestSet);
            createState = CreateState.ResourceRequestWait;
            break;
        case CreateState.ResourceRequestWait:
            if (resourceRequestSet.IsComplete () == true)
            {
                Object prefab = ResourceManager.Instance.GetAssetBundle ("cube");
                Debug.Log (prefab);
                instance = Object.Instantiate (prefab) as GameObject;
                instance.transform.SetParent (transform.parent);
                createState = CreateState.Complete;
            }
            break;
        case CreateState.Complete:
            createState = CreateState.ResourceRequest;
            return true;
        default:
            break;
        }
        return false;
    }

    public override void OnSceneDestroy ()
    {
        Debug.Log ("NextScene");
        Object.DestroyImmediate (instance);
        instance = null;
        ResourceManager.Instance.UnregisterRequestSet (RequestResourceLabel);
    }

    public void OnClickButton ()
    {
        Next (Scene.ExampleResourceManager_A, FadeType.Black, false);
    }
}
