using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
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
                EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type", "What kind of input we're detecting?"));
                if (inputType == TriggerOnInput.InputType.Button)
                {
                    EditorGUILayout.PropertyField(propButtonName, new GUIContent("Button Name", "Button name"));
                    EditorGUILayout.PropertyField(propContinuous, new GUIContent("Continuous", "If active, this triggers when the button is pressed, if not this trigger only executes when the key was just pressed."));
                }
                else if (inputType == TriggerOnInput.InputType.Key)
                {
                    EditorGUILayout.PropertyField(propKey, new GUIContent("Key", "Key name"));
                    EditorGUILayout.PropertyField(propContinuous, new GUIContent("Continuous", "If active, this triggers when the button is pressed, if not this trigger only executes when the key was just pressed."));
                }
                else if (inputType == TriggerOnInput.InputType.Axis)
                {
                    EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis", "Axis"));
                    EditorGUILayout.PropertyField(propDeadArea, new GUIContent("Dead Area", "How far from center does the axis have to go for it to execute this trigger?"));
                }
                if ((propContinuous.boolValue) || (inputType == TriggerOnInput.InputType.Axis))
                {
                    EditorGUILayout.PropertyField(propNegate, new GUIContent("Negate", "Do we want to trigger this when the input DOESN'T happen, instead of the other way around?"));
                }

                EditorGUILayout.PropertyField(propUseCooldown, new GUIContent("Use Cooldown", "Should we have a cooldown for this trigger?"));
                if (propUseCooldown.boolValue)
                {
                    EditorGUILayout.PropertyField(propCooldown, new GUIContent("Cooldown", "This trigger can only happen once every this time."));
                }

                EditorGUI.EndChangeCheck();

                ActionPanel();
            }
        }
    }
}