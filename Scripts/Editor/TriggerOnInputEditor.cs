using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerOnInput))]
public class TriggerOnInputEditor : TriggerEditor
{
    SerializedProperty propInputType;
    SerializedProperty propButtonName;
    SerializedProperty propKey;
    SerializedProperty propAxis;
    SerializedProperty propDeadArea;
    SerializedProperty propContinuous;
    SerializedProperty propNegate;
    SerializedProperty propUseCooldown;
    SerializedProperty propCooldown;

    protected override void OnEnable()
    {
        base.OnEnable();

        propInputType = serializedObject.FindProperty("inputType");
        propButtonName = serializedObject.FindProperty("buttonName");
        propKey = serializedObject.FindProperty("key");
        propAxis = serializedObject.FindProperty("axis");
        propDeadArea = serializedObject.FindProperty("deadArea");
        propContinuous = serializedObject.FindProperty("continuous");
        propNegate = serializedObject.FindProperty("negate");
        propUseCooldown = serializedObject.FindProperty("useCooldown");
        propCooldown = serializedObject.FindProperty("cooldown");
    }

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Input");

        return varTexture;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var trigger = (target as TriggerOnInput);
            if (trigger == null) return;

            TriggerOnInput.InputType inputType = (TriggerOnInput.InputType)propInputType.enumValueIndex;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type"));
            if (inputType == TriggerOnInput.InputType.Button)
            {
                EditorGUILayout.PropertyField(propButtonName, new GUIContent("Button Name"));
                EditorGUILayout.PropertyField(propContinuous, new GUIContent("Continuous"));
            }
            else if (inputType == TriggerOnInput.InputType.Key)
            {
                EditorGUILayout.PropertyField(propKey, new GUIContent("Key"));
                EditorGUILayout.PropertyField(propContinuous, new GUIContent("Continuous"));
            }
            else if (inputType == TriggerOnInput.InputType.Axis)
            {
                EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis"));
                EditorGUILayout.PropertyField(propDeadArea, new GUIContent("Dead Area"));
            }
            if ((propContinuous.boolValue) || (inputType == TriggerOnInput.InputType.Axis))
            {
                EditorGUILayout.PropertyField(propNegate, new GUIContent("Negate"));
            }

            EditorGUILayout.PropertyField(propUseCooldown, new GUIContent("Use Cooldown"));
            if (propUseCooldown.boolValue)
            {
                EditorGUILayout.PropertyField(propCooldown, new GUIContent("Cooldown"));
            }

            EditorGUI.EndChangeCheck();

            ActionPanel();
        }
    }
}
