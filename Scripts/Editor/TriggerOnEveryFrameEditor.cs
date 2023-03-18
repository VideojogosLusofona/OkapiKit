using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnEveryFrame))]
public class TriggerOnEveryFrameEditor : TriggerEditor
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Update");

        return varTexture;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            ActionPanel();
        }
    }
}
