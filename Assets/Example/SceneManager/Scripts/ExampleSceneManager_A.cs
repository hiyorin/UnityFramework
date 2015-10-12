using UnityEngine;
using Framework.Scene;

public class ExampleSceneManager_A : SceneBase
{
    public override bool OnSceneCreate ()
    {
        Debug.Log ("OnCreate " + "A");
        return true;
    }

    public override bool OnSceneInitialize ()
    {
        Debug.Log ("OnInitialize " + "A");
        return true;
    }

    public override void OnSceneStart ()
    {
        Debug.Log ("OnStart " + "A");
    }

    public override void OnSceneVisible ()
    {
        Debug.Log ("OnVisible " + "A");
    }

    public override void OnSceneUpdate ()
    {
        Debug.LogWarning ("OnUpdate " + "A");
    }

    public override void OnSceneInvisible ()
    {
        Debug.Log ("OnInvisible " + "A");
    }

    public override void OnSceneStop ()
    {
        Debug.Log ("OnStop " + "A");
    }

    public override void OnSceneDestroy ()
    {
        Debug.Log ("OnDestroy " + "A");
    }

    public override void OnSceneMessage (Scene owner, System.Collections.Generic.Dictionary<string, object> param)
    {
        if (owner == Scene.ExampleSceneManager_B)
        {
            string value = param ["ChangeSlide"] as string;
            if (value == "C")
            {
                ChangeSubSceneSlide (Scene.ExampleSceneManager_B, Scene.ExampleSceneManager_C, SlideType.ToLeft);
            }
        }
        else if (owner == Scene.ExampleSceneManager_C)
        {
            string value = param ["ChangeSlide"] as string;
            if (value == "B")
            {
                ChangeSubSceneSlide (Scene.ExampleSceneManager_C, Scene.ExampleSceneManager_B, SlideType.ToRight);
            }
        }
    }
}
