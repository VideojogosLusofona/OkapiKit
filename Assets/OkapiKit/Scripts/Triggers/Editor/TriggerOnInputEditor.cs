using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.MaterialProperty;

[CustomEditor(typeof(TriggerOnInput))]
public class TriggerOnInputEditor : TriggerEditor
{
    SerializedProperty propInputType;
    SerializedProperty propButtonName;
    SerializedProperty propKey;
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
        propContinuous = serializedObject.FindProperty("continuous");
        propNegate = serializedObject.FindProperty("negate");
        propUseCooldown = serializedObject.FindProperty("useCooldown");
        propCooldown = serializedObject.FindProperty("cooldown");
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
            } 
            else if (inputType == TriggerOnInput.InputType.Key)
            {
                EditorGUILayout.PropertyField(propKey, new GUIContent("Key"));
            }
            EditorGUILayout.PropertyField(propContinuous, new GUIContent("Continuous"));
            if (propContinuous.boolValue)
            {
                EditorGUILayout.PropertyField(propNegate, new GUIContent("Negate"));
                if (!propNegate.boolValue)
                {
                    EditorGUILayout.PropertyField(propUseCooldown, new GUIContent("Use Cooldown"));
                    if (propUseCooldown.boolValue)
                    {
                        EditorGUILayout.PropertyField(propCooldown, new GUIContent("Cooldown"));
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Trigger).UpdateExplanation();
            }

            ActionPanel();
        }
    }
}
