using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeValue_Deprecated))]
    public class ActionChangeValue_DeprecatedEditor : ActionEditor
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
                if ((propVariable.objectReferenceValue == null) && (propValueHandler.objectReferenceValue == null))
                {
                    EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Instance", "What's the value instance you want to set?\nYou can only choose either a value instance (instance on an object) or a variable (global value), not both at the same time."));
                    EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable", "What's the variable you want to set?\nYou can only choose either a value instance (instance on an object) or a variable (global value), not both at the same time."));
                }
                else if (propVariable.objectReferenceValue == null)
                {
                    EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Instance", "What's the value instance you want to set?\nYou can only choose either a value instance (instance on an object) or a variable (global value), not both at the same time."));
                }
                else
                {
                    EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable", "What's the variable you want to set?\nYou can only choose either a value instance (instance on an object) or a variable (global value), not both at the same time."));
                }
                EditorGUILayout.PropertyField(propOperation, new GUIContent("Operation", "What is the operation to perform on the variable/value instance?\nSet: Sets the variable to a certain value\nChange: Modifies the variable (add/subtract)\nReset: Sets a variable/value instance to its default value."));
                if (propOperation.intValue == (int)ActionChangeValue_Deprecated.OperationType.Change)
                {
                    EditorGUILayout.PropertyField(propDeltaValue, new GUIContent("Delta Value", "What is the value you want to add/subtract?"));
                    EditorGUILayout.PropertyField(propScaleWithTime, new GUIContent("Scale With Time", "Do you want to scale the value with time?\nThis is useful to have meters that go down a specific amount per second, for example."));
                }
                else if (propOperation.intValue == (int)ActionChangeValue_Deprecated.OperationType.Set)
                {
                    EditorGUILayout.PropertyField(propValue, new GUIContent("Value", "What is the value you want to set the value instance/variable?"));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as OkapiElement).UpdateExplanation();
                }
            }
        }
    }
}