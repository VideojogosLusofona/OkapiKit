using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TriggerOnCollision;

[CustomEditor(typeof(TriggerOnCondition))]
public class TriggerOnConditionEditor : TriggerEditor
{
    SerializedProperty propConditions;

    protected override void OnEnable()
    {
        base.OnEnable();

        propConditions = serializedObject.FindProperty("conditions");
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

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Trigger).UpdateExplanation();
            }

            ActionPanel();
        }
    }
}
