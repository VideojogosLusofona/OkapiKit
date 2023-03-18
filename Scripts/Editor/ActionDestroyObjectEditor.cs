using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionDestroyObject))]
public class ActionDestroyObjectEditor : ActionEditor
{
    SerializedProperty propTarget;
    SerializedProperty propTargetObject;
    SerializedProperty propTags;

    protected override void OnEnable()
    {
        base.OnEnable();

        propTarget = serializedObject.FindProperty("target");
        propTargetObject = serializedObject.FindProperty("targetObject");
        propTags = serializedObject.FindProperty("tags");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionDestroyObject);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));

            if (propTarget.enumValueIndex == (int)ActionDestroyObject.Target.Object)
            {
                EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target"));
            }
            else if (propTarget.enumValueIndex == (int)ActionDestroyObject.Target.Tag)
            {
                EditorGUILayout.PropertyField(propTags, new GUIContent("Tags"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
