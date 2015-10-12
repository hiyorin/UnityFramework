using UnityEngine;
using Framework.Scene;

public class ExampleSceneManager_B : SceneBase
{
    public override bool OnSceneCreate ()
    {
        Debug.Log ("OnCreate " + "B");
        return true;
    }

    public override bool OnSceneInitialize ()
    {
        Debug.Log ("OnInitialize " + "B");
        return true;
    }

    public override void OnSceneStart ()
    {
        Debug.Log ("OnStart " + "B");
    }

    public override void OnSceneVisible ()
    {
        Debug.Log ("OnVisible " + "B");
    }

    public override void OnSceneUpdate ()
    {
        Debug.LogWarning ("OnUpdate " + "B");
    }

    public override void OnSceneInvisible ()
    {
        Debug.Log ("OnInvisible " + "B");
    }

    public override void OnSceneStop ()
    {
        Debug.Log ("OnStop " + "B");
    }

    public override void OnSceneDestroy ()
    {
        Debug.Log ("OnDestroy " + "B");
    }

    public void OnClickButton ()
    {
        Next ("ExampleSceneManager_C", FadeType.Black, false);
    }

    public void OnClickButton_1 ()
    {
        SendSceneMessage (new System.Collections.Generic.Dictionary<string, object> {
            {"ChangeSlide", "C"}
        });
    }
}
