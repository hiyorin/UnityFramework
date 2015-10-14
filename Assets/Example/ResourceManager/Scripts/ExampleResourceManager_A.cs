using UnityEngine;
using UnityEngine.UI;
using Framework.Scene;
using Framework.Resource;

public class ExampleResourceManager_A : SceneBase
{
    private enum CreateState
    {
        ResourceRequest,
        ResourceRequestWait,
        Complete,
    }

    private readonly string RequestResourceLabel = "ExampleResourceManager_A";

    [SerializeField]
    private RawImage _imgCreate = null;

    private CreateState createState = CreateState.ResourceRequest;
    private ResourceRequestSet resourceRequestSet = new ResourceRequestSet ();

    public override bool OnSceneCreate ()
    {
        switch (createState)
        {
        case CreateState.ResourceRequest:
            resourceRequestSet.Add ("ExampleTexture", ResourceType.Asset);
            ResourceManager.Instance.RegisterRequestSet (
                RequestResourceLabel, resourceRequestSet);
            createState = CreateState.ResourceRequestWait;
            break;
        case CreateState.ResourceRequestWait:
            if (resourceRequestSet.IsComplete () == true)
                createState = CreateState.Complete;
            break;
        case CreateState.Complete:
            createState = CreateState.ResourceRequest;
            return true;
        default:
            break;
        }
        return false;
    }

    public override void OnSceneStart ()
    {
        Texture2D exampleTexture = ResourceManager.Instance.GetAsset ("ExampleTexture") as Texture2D;
        _imgCreate.texture = exampleTexture;
        _imgCreate.SetNativeSize ();
    }

    public override void OnSceneDestroy ()
    {
        ResourceManager.Instance.UnregisterRequestSet (RequestResourceLabel);
    }
    
    public void OnClickSlideOut ()
    {
        ChangeSubSceneSlide (Scene.ExampleResourceManager_B, Scene.Empty, SlideType.ToLeft);
    }

    public void OnClickSlideIn ()
    {
        ChangeSubSceneSlide (Scene.Empty, Scene.ExampleResourceManager_B, SlideType.ToLeft);
    }
}
