using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeResource))]
    public class ActionChangeResourceEditor : ActionEditor
    {
        SerializedProperty resource;
        SerializedProperty operation;
        SerializedProperty changeValue;

        protected override void OnEnable()
        {
            base.OnEnable();

            resource = serializedObject.FindProperty("resource");
            operation = serializedObject.FindProperty("operation");
            changeValue = serializedObject.FindProperty("changeValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(resource, new GUIContent("Target Resource", "What's the resource to change value?"));
                EditorGUILayout.PropertyField(operation, new GUIContent("Operation", "Operation to run over the resource"));
                EditorGUILayout.PropertyField(changeValue, new GUIContent("Value", "Value to change resource"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}
