using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeValue))]
    public class ActionChangeValueEditor : ActionEditor
    {
        SerializedProperty propValueHandler;
        SerializedProperty propVariable;
        SerializedProperty propOperation;
        SerializedProperty propDeltaValue;
        SerializedProperty propScaleWithTime;
        SerializedProperty propValue;

        protected override void OnEnable()
        {
            base.OnEnable();

            propValueHandler = serializedObject.FindProperty("valueHandler");
            propVariable = serializedObject.FindProperty("variable");
            propOperation = serializedObject.FindProperty("operation");
            propDeltaValue = serializedObject.FindProperty("deltaValue");
            propScaleWithTime = serializedObject.FindProperty("scaleWithTime");
            propValue = serializedObject.FindProperty("value");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propOperation, new GUIContent("Operation"));
                if ((propVariable.objectReferenceValue == null) && (propValueHandler.objectReferenceValue == null))
                {
                    EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Handler"));
                    EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable"));
                }
                else if (propVariable.objectReferenceValue == null)
                {
                    EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Handler"));
                }
                else
                {
                    EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable"));
                }
                if (propOperation.enumValueIndex == (int)ActionChangeValue.OperationType.Change)
                {
                    EditorGUILayout.PropertyField(propDeltaValue, new GUIContent("Delta Value"));
                    EditorGUILayout.PropertyField(propScaleWithTime, new GUIContent("Scale With Time"));
                }
                else if (propOperation.enumValueIndex == (int)ActionChangeValue.OperationType.Set)
                {
                    EditorGUILayout.PropertyField(propValue, new GUIContent("Value"));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}