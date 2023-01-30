using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnReset))]
public class TriggerOnResetEditor : TriggerEditor
{
    protected override void OnEnable()
    {
        base.OnEnable();
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
