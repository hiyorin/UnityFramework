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

    public override bool OnSceneCreate ()
    {
        switch (createState)
        {
        case CreateState.ResourceRequest:
            resourceRequestSet.Add ("prefab", ResourceType.AssetBundle);
            ResourceManager.Instance.RegisterRequestSet (
                RequestResourceLabel, resourceRequestSet);
            createState = CreateState.ResourceRequestWait;
            break;
        case CreateState.ResourceRequestWait:
            if (resourceRequestSet.IsComplete () == true)
            {
                Object prefab = ResourceManager.Instance.GetAssetBundle ("prefab");
                Object.Instantiate (prefab);
                createState = CreateState.Complete;
            }
            break;
        case CreateState.Complete:
            createState = CreateState.ResourceRequest;
            break;
        default:
            break;
        }
        return true;
    }
}
