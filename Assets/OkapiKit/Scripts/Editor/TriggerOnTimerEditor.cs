using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnTimer))]
public class TriggerOnTimerEditor : TriggerEditor
{
    SerializedProperty propStartTriggered;
    SerializedProperty propTimeInterval;

    protected override void OnEnable()
    {
        base.OnEnable();

        propStartTriggered = serializedObject.FindProperty("startTriggered");
        propTimeInterval = serializedObject.FindProperty("timeInterval");
    }

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Timer");

        return varTexture;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var trigger = (target as TriggerOnTimer);
            if (trigger == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propStartTriggered, new GUIContent("Trigger At Start?"));
            EditorGUILayout.PropertyField(propTimeInterval, new GUIContent("Time interval (random between [X..Y]"));

            EditorGUI.EndChangeCheck();

            ActionPanel();
        }
    }
}
