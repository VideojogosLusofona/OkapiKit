using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Trigger_OnCondition.Condition))]
public class ConditionDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width, position.height * 2), label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Check if we need both selectors
        var propValueHandler = property.FindPropertyRelative("valueHandler");
        var propVariable = property.FindPropertyRelative("variable");
        var propComparison = property.FindPropertyRelative("comparison");
        var propValue = property.FindPropertyRelative("value");
        var propPercentage = property.FindPropertyRelative("percentageCompare");

        float offset_y = 0.0f;
        float height = position.height;
        var width = position.width;
        var extra_width = (width - 150 - 110 - 60 - 20 - 30);
        var extra_width_variable = extra_width * 2.0f / 3.0f;
        var extra_width_value = extra_width / 3.0f;

        if (propValueHandler.objectReferenceValue == null)
        {
            if (propVariable.objectReferenceValue == null)
            {
                // Calculate rects
                var valueHandlerRect = new Rect(position.x, position.y, 150 + extra_width_variable, position.height / 2);
                var variableRect = new Rect(position.x, position.y + position.height / 2, 150 + extra_width_variable, position.height / 2);

                // Draw fields - pass GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(valueHandlerRect, propValueHandler, GUIContent.none);
                EditorGUI.PropertyField(variableRect, propVariable, GUIContent.none);

                offset_y = position.height / 4;
            }
            else
            {
                var variableRect = new Rect(position.x, position.y, 150 + extra_width_variable, position.height);
                EditorGUI.PropertyField(variableRect, propVariable, GUIContent.none);
            }
        }
        else
        {
            var valueHandlerRect = new Rect(position.x, position.y, 150 + extra_width_variable, position.height);
            EditorGUI.PropertyField(valueHandlerRect, propValueHandler, GUIContent.none);
        }

        var propComparisonRect = new Rect(position.x + 155 + extra_width_variable, position.y + offset_y, 110, height);
        var propValueRect = new Rect(propComparisonRect.max.x + 5, position.y, 60 + extra_width_value, height);
        var propPercentageLabelRect = new Rect(propValueRect.max.x + 10, position.y + offset_y, 20, height);
        var propPercentageRect = new Rect(propPercentageLabelRect.max.x, position.y + offset_y, 30, height);

        EditorGUI.PropertyField(propComparisonRect, propComparison, GUIContent.none);
        EditorGUI.PropertyField(propValueRect, propValue, GUIContent.none);
        EditorGUI.LabelField(propPercentageLabelRect, "%");
        EditorGUI.PropertyField(propPercentageRect, propPercentage, GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var propValueHandler = property.FindPropertyRelative("valueHandler");
        var propVariable = property.FindPropertyRelative("variable");

        if (propValueHandler.objectReferenceValue == null)
        {
            if (propVariable.objectReferenceValue == null)
            {
                return base.GetPropertyHeight(property, label) * 2;
            }
        }

        return base.GetPropertyHeight(property, label);
    }
}
