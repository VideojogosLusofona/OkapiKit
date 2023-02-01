using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TriggerOnCollision;

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

    public override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("TimerTexture");
        if (varTexture == null)
        {
            varTexture = GUIUtils.AddTexture("TimerTexture", new CodeBitmaps.Timer());
        }

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

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Trigger).UpdateExplanation();
            }

            ActionPanel();
        }
    }
}