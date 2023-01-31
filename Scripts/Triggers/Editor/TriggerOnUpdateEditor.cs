using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnUpdate))]
public class TriggerOnUpdateEditor : TriggerEditor
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("UpdateTexture");
        if (varTexture == null)
        {
            varTexture = GUIUtils.AddTexture("UpdateTexture", new CodeBitmaps.Update());
        }

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
