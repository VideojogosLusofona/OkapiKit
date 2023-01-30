using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionTagged))]
public class ActionTaggedEditor : ActionEditor
{
    SerializedProperty propSearchType;
    SerializedProperty propSearchTags;
    SerializedProperty propTriggerType;
    SerializedProperty propTriggerTags;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSearchType = serializedObject.FindProperty("searchType");
        propSearchTags = serializedObject.FindProperty("searchTags");
        propTriggerType = serializedObject.FindProperty("triggerType");
        propTriggerTags = serializedObject.FindProperty("triggerTags");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionTagged);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propSearchType, new GUIContent("Search Type"));
            EditorGUILayout.PropertyField(propSearchTags, new GUIContent("Search Tags"));
            EditorGUILayout.PropertyField(propTriggerType, new GUIContent("Trigger Type"));
            EditorGUILayout.PropertyField(propTriggerTags, new GUIContent("Trigger Tags"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
