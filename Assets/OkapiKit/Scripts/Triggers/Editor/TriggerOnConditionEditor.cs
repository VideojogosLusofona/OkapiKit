using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnCondition))]
public class TriggerOnConditionEditor : TriggerEditor
{
    SerializedProperty propConditions;

    protected override void OnEnable()
    {
        base.OnEnable();

        propConditions = serializedObject.FindProperty("conditions");
    }

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Condition");

        return varTexture;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var trigger = (target as TriggerOnCondition);
            if (trigger == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propConditions, new GUIContent("Conditions"));

            EditorGUI.EndChangeCheck();

            ActionPanel();
        }
    }
}
