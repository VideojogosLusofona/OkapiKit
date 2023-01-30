using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class ActionChangeScene : Action
{
    [SerializeField, Scene] private string sceneName;

    public override string GetActionTitle() => "Change Scene";

    public override string GetRawDescription(string ident)
    {
        return $"{GetPreconditionsString()} switches to scene {sceneName}";
    }

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        SceneManager.LoadScene(sceneName);
    }
}
