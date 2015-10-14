using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.Scene;
using Framework.Resource;

public class ExampleResourceManager_B : SceneBase
{

    private readonly string RequestResourceLabel = "ExampleResourceManager_B";

    [SerializeField]
    private GameObject _prefab = null;
    [SerializeField]
    private LayoutGroup _layout = null;

    private ResourceRequestSet resourceRequestSet = null;
    private List<string> listShowedResourceName = new List<string> ();
    
    public override void OnSceneVisible ()
    {
        Debug.Log ("OnSceneVisible_B");
        resourceRequestSet = new ResourceRequestSet ();
        for (var i=0; i<100; i++)
        {
            resourceRequestSet.Add (
                string.Format ("http://pictogram-free.com/thumbnail/s_{0}.png", (i+1).ToString ("000")),
                ResourceType.Texture);
        }
        ResourceManager.Instance.RegisterRequestSet (
            RequestResourceLabel,
            resourceRequestSet);
    }

    public override void OnSceneUpdate ()
    {
        if (listShowedResourceName.Count == resourceRequestSet.Count ())
            return;
        
        foreach (var row in resourceRequestSet.GetList ())
        {
            if (listShowedResourceName.Contains (row.url) == true)
                continue;
            Texture2D texture = ResourceManager.Instance.GetTexture (row.url);
            if (texture == null)
                continue;
            listShowedResourceName.Add (row.url);

            RawImage img = GameObject.Instantiate (_prefab).GetComponent<RawImage> ();
            img.gameObject.SetActive (true);
            img.transform.SetParent (_layout.transform);
            img.transform.Reset ();
            img.texture = texture;
        }
    }

    public override void OnSceneInvisible ()
    {
        Debug.Log ("OnSceneInvisible_B");
        ResourceManager.Instance.UnregisterRequestSet (RequestResourceLabel);
        listShowedResourceName.Clear ();

        foreach (Transform trans in _layout.transform)
        {
            if (trans.gameObject.activeSelf == true)
                Object.Destroy (trans.gameObject);
        }

        resourceRequestSet = null;
    }
}
