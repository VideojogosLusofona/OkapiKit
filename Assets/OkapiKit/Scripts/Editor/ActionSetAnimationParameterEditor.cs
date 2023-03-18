using UnityEditor;
using UnityEngine;

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
            EditorGUILayout.PropertyField(propAnimator, new GUIContent("Animator"));
            EditorGUILayout.PropertyField(propAnimationParameter, new GUIContent("Parameter"));
            EditorGUILayout.PropertyField(propValueType, new GUIContent("Type"));

            var type = (ActionSetAnimationParameter.ValueType)propValueType.enumValueIndex;
            switch (type)
            {
                case ActionSetAnimationParameter.ValueType.Int:
                    EditorGUILayout.PropertyField(propIntegerValue, new GUIContent("Value"));
                    break;
                case ActionSetAnimationParameter.ValueType.Float:
                    EditorGUILayout.PropertyField(propFloatValue, new GUIContent("Value"));
                    break;
                case ActionSetAnimationParameter.ValueType.Boolean:
                    EditorGUILayout.PropertyField(propBoolValue, new GUIContent("Value"));
                    break;
                case ActionSetAnimationParameter.ValueType.Trigger:
                    break;
                case ActionSetAnimationParameter.ValueType.Value:
                    if (propValueHandler.objectReferenceValue == null)
                    {
                        if (propVariable.objectReferenceValue == null)
                        {
                            EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Handler"));
                            EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable"));
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(propVariable, new GUIContent("Variable"));
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propValueHandler, new GUIContent("Value Handler"));
                    }
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
