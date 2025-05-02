using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeValueV2))]
    public class ActionChangeValueV2Editor : ActionEditor
    {
        SerializedProperty propValueHandler;
        SerializedProperty propVariable;
        SerializedProperty propOperation;
        SerializedProperty propChangeValueHandler;
        SerializedProperty propChangeVariable;
        SerializedProperty propChangeValue;
        SerializedProperty propScaleWithTime;

        protected override void OnEnable()
        {
            base.OnEnable();

            propValueHandler = serializedObject.FindProperty("valueHandler");
            propVariable = serializedObject.FindProperty("variable");
            propOperation = serializedObject.FindProperty("operation");
            propChangeValue = serializedObject.FindProperty("changeValue");
            propChangeValueHandler = serializedObject.FindProperty("changeValueHandler");
            propChangeVariable = serializedObject.FindProperty("changeVariable");
            propScaleWithTime = serializedObject.FindProperty("scaleWithTime");
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
                EditorGUILayout.PropertyField(propOperation, new GUIContent("Operation", "What is the operation to perform on the variable/value instance?\nReset: Sets a variable/value instance to its default value.\nSet: Sets the variable to a certain value\nAdd: Adds a value to a variable\nSubtract: Subtracts a value from a variable\nRevSubtract: Subtracts this variable from a value\nMultiply: Multiplies the variable by a value\nDivide: Divides the variable by a value\nRevDivide: Divides a value by this variable"));

                var opType = (OperationType)propOperation.intValue;

                if (opType == OperationType.Reset)
                {

                }
                else if (opType == OperationType.Set)
                {
                    ShowValue();
                }
                else
                {
                    ShowValue();
                    EditorGUILayout.PropertyField(propScaleWithTime, new GUIContent("Scale With Time", "Do you want to scale the value with time?\nThis is useful to have meters that go down a specific amount per second, for example."));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }

        void ShowValue()
        {
            if ((propChangeVariable.objectReferenceValue == null) && (propChangeValueHandler.objectReferenceValue == null))
            {
                EditorGUILayout.PropertyField(propChangeValueHandler, new GUIContent("Change Value Instance", "What's the value you want to use in the operation?\nYou can only choose either a value instance (instance on an object), a variable (global value), or a direct value."));
                EditorGUILayout.PropertyField(propChangeVariable, new GUIContent("Change Variable", "What's the value you want to use in the operation?\nYou can only choose either a value instance (instance on an object), a variable (global value), or a direct value."));
                EditorGUILayout.PropertyField(propChangeValue, new GUIContent("Change Value", "What's the value you want to use in the operation?\nYou can only choose either a value instance (instance on an object), a variable (global value), or a direct value."));
            }
            else if (propChangeVariable.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(propChangeValueHandler, new GUIContent("Change Value Instance", "What's the value you want to use in the operation?\nYou can only choose either a value instance (instance on an object), a variable (global value), or a direct value."));
            }
            else
            {
                EditorGUILayout.PropertyField(propChangeVariable, new GUIContent("Change Variable", "What's the value you want to use in the operation?\nYou can only choose either a value instance (instance on an object), a variable (global value), or a direct value."));
            }
        }
    }
}
