using UnityEngine;
using Framework.Scene;

public class ExampleSceneManager_C : SceneBase
{
    public override bool OnSceneCreate ()
    {
        Debug.Log ("OnCreate " + "C");
        return true;
    }

    public override bool OnSceneInitialize ()
    {
        Debug.Log ("OnInitialize " + "C");
        return true;
    }

    public override void OnSceneStart ()
    {
        Debug.Log ("OnStart " + "C");
    }

    public override void OnSceneVisible ()
    {
        Debug.Log ("OnVisible " + "C");
    }

    public override void OnSceneUpdate ()
    {
        Debug.LogWarning ("OnUpdate " + "C");
    }

    public override void OnSceneInvisible ()
    {
        Debug.Log ("OnInvisible " + "C");
    }

    public override void OnSceneStop ()
    {
        Debug.Log ("OnStop " + "C");
    }

    public override void OnSceneDestroy ()
    {
        Debug.Log ("OnDestroy " + "C");
    }

    public void OnClickButton ()
    {
        Next (Scene.ExampleSceneManager_B, FadeType.White, false);
    }

    public void OnClickButton_1 ()
    {
        SendSceneMessage (new System.Collections.Generic.Dictionary<string, object> () {
            { "ChangeSlide", "B" }
        });
    }
}
