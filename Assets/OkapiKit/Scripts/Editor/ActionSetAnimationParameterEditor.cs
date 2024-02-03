using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionSetAnimationParameter))]
    public class ActionSetAnimationParameterEditor : ActionEditor
    {
        SerializedProperty propAnimator;
        SerializedProperty propAnimationParameter;
        SerializedProperty propValueType;
        SerializedProperty propIntegerValue;
        SerializedProperty propFloatValue;
        SerializedProperty propBoolValue;
        SerializedProperty propValueHandler;
        SerializedProperty propVariable;
        SerializedProperty propAbsolute;

        protected override void OnEnable()
        {
            base.OnEnable();

            propAnimator = serializedObject.FindProperty("animator");
            propAnimationParameter = serializedObject.FindProperty("animationParameter");
            propValueType = serializedObject.FindProperty("valueType");
            propIntegerValue = serializedObject.FindProperty("integerValue");
            propFloatValue = serializedObject.FindProperty("floatValue");
            propBoolValue = serializedObject.FindProperty("boolValue");
            propValueHandler = serializedObject.FindProperty("valueHandler");
            propVariable = serializedObject.FindProperty("variable");
            propAbsolute = serializedObject.FindProperty("absolute");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionSetAnimationParameter);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propAnimator, new GUIContent("Animator", "What is the target animator?"));
                EditorGUILayout.PropertyField(propAnimationParameter, new GUIContent("Parameter", "What parameter do you want to change?"));
                EditorGUILayout.PropertyField(propValueType, new GUIContent("Type", "What data type do you want to change?"));

                var type = (ActionSetAnimationParameter.ValueType)propValueType.enumValueIndex;
                switch (type)
                {
                    case ActionSetAnimationParameter.ValueType.Int:
                        EditorGUILayout.PropertyField(propIntegerValue, new GUIContent("Value", "Integer value to set"));
                        break;
                    case ActionSetAnimationParameter.ValueType.Float:
                        EditorGUILayout.PropertyField(propFloatValue, new GUIContent("Value", "Float value to set"));
                        break;
                    case ActionSetAnimationParameter.ValueType.Boolean:
                        EditorGUILayout.PropertyField(propBoolValue, new GUIContent("Value", "Boolean value to set"));
                        break;
                    case ActionSetAnimationParameter.ValueType.Trigger:
                        break;
                    case ActionSetAnimationParameter.ValueType.Value:
                        if (propValueHandler.objectReferenceValue == null)
                        {
                            if (propVariable.objectReferenceValue == null)
                            {
                                EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Instance", "Value handler to set to parameter.\nYou can select a value instance or a variable, but not both at the same time."));
                                EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable", "Variable to set to parameter.\nYou can select a value instance or a variable, but not both at the same time."));
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable", "Variable to set to parameter.\nYou can select a value instance or a variable, but not both at the same time."));
                            }
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Instance", "Value handler to set to parameter.\nYou can select a value instance or a variable, but not both at the same time."));
                        }
                        break;
                    case ActionSetAnimationParameter.ValueType.VelocityX:
                    case ActionSetAnimationParameter.ValueType.VelocityY:
                        EditorGUILayout.PropertyField(propAbsolute, new GUIContent("Use absolute value?", "If active, use the absolute velocity (non-negative)"));
                        break;
                    default:
                        break;
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