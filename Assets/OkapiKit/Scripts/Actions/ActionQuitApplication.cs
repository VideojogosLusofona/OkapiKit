using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ActionQuitApplication : Action
{
    public override string GetRawDescription(string ident)
    {
        return "Quit application";
    }

    public override void Execute()
    {
        if (!enableAction) return;

#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }
}
