using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class ActionChangeScene : Action
{
    [SerializeField, Scene] private string sceneName;

    public override string GetRawDescription(string ident)
    {
        return $"Switches to scene {sceneName}";
    }

    public override void Execute()
    {
        if (!enableAction) return;

        SceneManager.LoadScene(sceneName);
    }
}
