using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace OkapiKit.Editor
{

    [CustomPropertyDrawer(typeof(InputControl))]
    public class InputControlDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Indent and calculate position
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Define the height of each row
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // Get properties
            var typeProp = property.FindPropertyRelative("_type");
            var axisProp = property.FindPropertyRelative("axis");
            var buttonPositiveProp = property.FindPropertyRelative("buttonPositive");
            var buttonNegativeProp = property.FindPropertyRelative("buttonNegative");
            var keyPositiveProp = property.FindPropertyRelative("keyPositive");
            var keyNegativeProp = property.FindPropertyRelative("keyNegative");

            // Get custom attributes
            var allowInputAttr = (AllowInputAttribute)fieldInfo.GetCustomAttribute(typeof(AllowInputAttribute), true);
            var inputButtonAttr = (InputButtonAttribute)fieldInfo.GetCustomAttribute(typeof(InputButtonAttribute), true);

            // Determine allowed input types
            AllowInput allowedTypes = allowInputAttr != null ? allowInputAttr.AllowedInputs : AllowInput.All;

            // Show only the allowed input types in the enum popup
            Rect typeRect = new Rect(position.x, position.y, position.width, singleLineHeight);
            InputControl.InputType selectedType = DrawFilteredEnumPopup(typeRect, typeProp, allowedTypes);

            // Adjust label width temporarily
            float originalLabelWidth = EditorGUIUtility.labelWidth;

            // Display the appropriate fields based on selected InputType
            switch (selectedType)
            {
                case InputControl.InputType.Axis:
                    EditorGUIUtility.labelWidth = CalcLabelWidth("Axis");
                    Rect axisRect = new Rect(position.x, position.y + singleLineHeight + spacing, position.width, singleLineHeight);
                    EditorGUI.PropertyField(axisRect, axisProp, new GUIContent("Axis"));
                    break;

                case InputControl.InputType.Button:
                    EditorGUIUtility.labelWidth = Mathf.Max(CalcLabelWidth("Positive Button"), CalcLabelWidth("Negative Button"));
                    Rect buttonPositiveRect = new Rect(position.x, position.y + singleLineHeight + spacing, position.width, singleLineHeight);

                    // If not a button-only control, show the negative button as well
                    if (inputButtonAttr == null)
                    {
                        EditorGUI.PropertyField(buttonPositiveRect, buttonPositiveProp, new GUIContent("Positive Button"));
                        Rect buttonNegativeRect = new Rect(position.x, position.y + 2 * (singleLineHeight + spacing), position.width, singleLineHeight);
                        EditorGUI.PropertyField(buttonNegativeRect, buttonNegativeProp, new GUIContent("Negative Button"));
                    }
                    else
                    {
                        EditorGUI.PropertyField(buttonPositiveRect, buttonPositiveProp, new GUIContent("Button"));
                    }
                    break;

                case InputControl.InputType.Key:
                    EditorGUIUtility.labelWidth = Mathf.Max(CalcLabelWidth("Positive Key"), CalcLabelWidth("Negative Key"));
                    Rect keyPositiveRect = new Rect(position.x, position.y + singleLineHeight + spacing, position.width, singleLineHeight);

                    // If not a button-only control, show the negative button as well
                    if (inputButtonAttr == null)
                    {
                        EditorGUI.PropertyField(keyPositiveRect, keyPositiveProp, new GUIContent("Positive Key"));
                        Rect keyNegativeRect = new Rect(position.x, position.y + 2 * (singleLineHeight + spacing), position.width, singleLineHeight);
                        EditorGUI.PropertyField(keyNegativeRect, keyNegativeProp, new GUIContent("Negative Key"));
                    }
                    else
                    {
                        EditorGUI.PropertyField(keyPositiveRect, keyPositiveProp, new GUIContent("Key"));
                    }
                    break;
            }

            // Restore the original label width
            EditorGUIUtility.labelWidth = originalLabelWidth;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private InputControl.InputType DrawFilteredEnumPopup(Rect position, SerializedProperty typeProp, AllowInput allowedTypes)
        {
            var enumNames = System.Enum.GetNames(typeof(InputControl.InputType));
            var enumValues = System.Enum.GetValues(typeof(InputControl.InputType));
            var options = new List<GUIContent>();
            var filteredIndices = new List<int>();

            for (int i = 0; i < enumNames.Length; i++)
            {
                var inputType = (InputControl.InputType)enumValues.GetValue(i);
                if ((allowedTypes & (AllowInput)(1 << i)) != 0)
                {
                    options.Add(new GUIContent(enumNames[i]));
                    filteredIndices.Add(i);
                }
            }

            int currentIndex = filteredIndices.IndexOf(typeProp.enumValueIndex);
            currentIndex = EditorGUI.Popup(position, currentIndex, options.ToArray());

            if (currentIndex >= 0)
            {
                typeProp.enumValueIndex = filteredIndices[currentIndex];
            }

            return (InputControl.InputType)typeProp.enumValueIndex;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeProp = property.FindPropertyRelative("_type");
            InputControl.InputType type = (InputControl.InputType)typeProp.enumValueIndex;

            // Check for InputButton attribute to determine if it's button-only
            bool isButtonOnly = fieldInfo.GetCustomAttribute<InputButtonAttribute>() != null;

            // Calculate height based on the selected InputType and button-only setting
            int rows = 1; // Start with 1 for the enum field
            switch (type)
            {
                case InputControl.InputType.Axis:
                    rows += 1;
                    break;
                case InputControl.InputType.Button:
                case InputControl.InputType.Key:
                    rows += isButtonOnly ? 1 : 2;
                    break;
            }

            return rows * EditorGUIUtility.singleLineHeight + (rows - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        // Helper method to calculate the label width based on text
        private float CalcLabelWidth(string labelText)
        {
            return GUI.skin.label.CalcSize(new GUIContent(labelText)).x + 5;
        }
    }
}
