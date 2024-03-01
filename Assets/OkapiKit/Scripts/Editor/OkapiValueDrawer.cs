using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomPropertyDrawer(typeof(OkapiValue))]
    public class OkapiValueDrawer : PropertyDrawer
    {
        bool HasAttribute<T>() where T : Attribute
        {
            T attr = attribute as T ?? fieldInfo.GetCustomAttribute<T>();

            return attr != null;
        }

        bool labelEnabled => !HasAttribute<OVNoLabelAttribute>();
        bool floatEnabled => !HasAttribute<OVNoFloatAttribute>();
        bool integerEnabled => !HasAttribute<OVNoIntegerAttribute>();
        bool variableInstanceEnabled => !HasAttribute<OVNoVariableInstanceAttribute>();
        bool variableEnabled => !HasAttribute<OVNoVariableAttribute>();

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width, position.height * 2), label, property);

            // Draw label - NO LABEL!
            if (labelEnabled)
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            }

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var propInit = property.FindPropertyRelative("init");
            var propType = property.FindPropertyRelative("type");
            var propFloat = property.FindPropertyRelative("floatValue");
            var propInt = property.FindPropertyRelative("intValue");
            var propVariableInstance = property.FindPropertyRelative("variableInstance");
            var propVariable = property.FindPropertyRelative("variable");

            // Might need to initialize the value with some defaults
            if (!propInit.boolValue)
            {
                propInit.boolValue = true;

                if (floatEnabled)
                {
                    propType.intValue = (int)OkapiValue.Type.Float;
                    propFloat.floatValue = 0.0f;
                }
                else if (integerEnabled)
                {
                    propType.intValue = (int)OkapiValue.Type.Integer;
                    propInt.intValue = 0;
                }
                else if (variableInstanceEnabled)
                {
                    propType.intValue = (int)OkapiValue.Type.VariableInstance;
                    propVariableInstance.objectReferenceValue = null;
                }
                else if (variableEnabled)
                {
                    propType.intValue = (int)OkapiValue.Type.Variable;
                    propVariable.objectReferenceValue = null;
                }

                // Mark the entire object as dirty to ensure changes are saved
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                // Apply the modified properties to ensure changes are serialized
                property.serializedObject.ApplyModifiedProperties();
            }

            var typeRect = position;
            typeRect.width = typeRect.width * 0.25f;

            GetAvailableOptions(out var typeOptionStrings, out var typeOptionValues);

            int currentIndex = Array.IndexOf(typeOptionValues, (OkapiValue.Type)propType.intValue);

            int newIndex = EditorGUI.Popup(typeRect, GUIContent.none, currentIndex, typeOptionStrings);

            if (newIndex != currentIndex)
            {
                // Update the actual value on the script based on the selected option
                propType.intValue = (int)typeOptionValues[newIndex];

                // Mark the entire object as dirty to ensure changes are saved
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                // Apply the modified properties to ensure changes are serialized
                property.serializedObject.ApplyModifiedProperties();
            }

            var valueRect = position;
            valueRect.x = typeRect.xMax;
            valueRect.width = valueRect.width * 0.75f;

            var type = (OkapiValue.Type)propType.intValue;
            switch (type)
            {
                case OkapiValue.Type.Float:
                    EditorGUI.PropertyField(valueRect, propFloat, GUIContent.none);
                    break;
                case OkapiValue.Type.Integer:
                    EditorGUI.PropertyField(valueRect, propInt, GUIContent.none);
                    break;
                case OkapiValue.Type.VariableInstance:
                    EditorGUI.PropertyField(valueRect, propVariableInstance, GUIContent.none);
                    break;
                case OkapiValue.Type.Variable:
                    EditorGUI.PropertyField(valueRect, propVariable, GUIContent.none);
                    break;
                default:
                    break;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = base.GetPropertyHeight(property, label) + 2;

            return baseHeight;
        }

        void GetAvailableOptions(out GUIContent[] typeOptionStrings, out OkapiValue.Type[] typeOptionValues)
        {
            var listTypeOptionStrings = new List<GUIContent>();
            var listTypeOptionValues = new List<OkapiValue.Type>();

            if (floatEnabled) { listTypeOptionStrings.Add(new GUIContent("Float")); listTypeOptionValues.Add(OkapiValue.Type.Float); }
            if (integerEnabled) { listTypeOptionStrings.Add(new GUIContent("Integer")); listTypeOptionValues.Add(OkapiValue.Type.Integer); }
            if (variableInstanceEnabled) { listTypeOptionStrings.Add(new GUIContent("Variable Instance")); listTypeOptionValues.Add(OkapiValue.Type.VariableInstance); }
            if (variableEnabled) { listTypeOptionStrings.Add(new GUIContent("Variable")); listTypeOptionValues.Add(OkapiValue.Type.Variable); }

            typeOptionStrings = listTypeOptionStrings.ToArray();
            typeOptionValues = listTypeOptionValues.ToArray();
        }
    }
}