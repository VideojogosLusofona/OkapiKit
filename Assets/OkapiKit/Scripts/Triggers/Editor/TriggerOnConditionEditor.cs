using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnCondition))]
public class TriggerOnConditionEditor : TriggerEditor
{
    SerializedProperty propConditions;
    SerializedProperty propElseActions;

    protected override void OnEnable()
    {
        base.OnEnable();

        propConditions = serializedObject.FindProperty("conditions");
        propElseActions = serializedObject.FindProperty("elseActions");
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
            StdEditor(false, false);

            var trigger = (target as TriggerOnCondition);
            if (trigger == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propConditions, new GUIContent("Conditions"));

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            serializedObject.Update();
            EditorGUILayout.PropertyField(propActions, new GUIContent("Actions"), true);
            EditorGUILayout.PropertyField(propElseActions, new GUIContent("Else Actions"), true);

            serializedObject.ApplyModifiedProperties();
            (target as Trigger).UpdateExplanation();
        }
    }
}
