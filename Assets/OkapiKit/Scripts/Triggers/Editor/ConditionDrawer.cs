using NaughtyAttributes.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width, position.height * 2), label, property);

        // Draw label - NO LABEL!
        //label.text = label.text.Replace("Element", "Condition");
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Check if we need both selectors
        var propNegate = property.FindPropertyRelative("negate");
        var propValueHandler = property.FindPropertyRelative("valueHandler");
        var propVariable = property.FindPropertyRelative("variable");
        var propValueType = property.FindPropertyRelative("valueType");
        var propTag = property.FindPropertyRelative("tag");
        var propTransform = property.FindPropertyRelative("sourceTransform");
        var propRB = property.FindPropertyRelative("rigidBody");
        var propAxis = property.FindPropertyRelative("axis");
        var propProbe = property.FindPropertyRelative("probe");

        var propComparison = property.FindPropertyRelative("comparison");
        var propValue = property.FindPropertyRelative("value");
        var propPercentage = property.FindPropertyRelative("percentageCompare");

        float positionValue = position.x + 50;

        EditorGUI.LabelField(new Rect(position.x, position.y, 50, position.height), "Not");
        propNegate.boolValue = EditorGUI.Toggle(new Rect(position.x + 25, position.y, 20, position.height), propNegate.boolValue);

        float offset_y = 0.0f;
        float height = position.height;
        var width = position.width - 50;
        var extra_width = (width - 150 - 110 - 60 - 20 - 30);
        var extra_width_variable = extra_width * 2.0f / 3.0f;
        var extra_width_value = extra_width / 3.0f;

        Condition.DataType dataType = Condition.DataType.Number;

        if (propValueHandler.objectReferenceValue == null)
        {
            if (propVariable.objectReferenceValue == null)
            {
                if (propValueType.enumValueIndex == 0)
                {
                    // Calculate rects
                    var valueHandlerRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height / 3);
                    var variableRect = new Rect(positionValue, position.y + position.height / 3, 150 + extra_width_variable, position.height / 3);
                    var systemRect = new Rect(positionValue, position.y + 2 * position.height / 3, 150 + extra_width_variable, position.height / 3);

                    // Draw fields - pass GUIContent.none to each so they are drawn without labels
                    EditorGUI.PropertyField(valueHandlerRect, propValueHandler, GUIContent.none);
                    EditorGUI.PropertyField(variableRect, propVariable, GUIContent.none);
                    EditorGUI.PropertyField(systemRect, propValueType, GUIContent.none);

                    offset_y = position.height / 4;
                }
                else
                {
                    Condition.ValueType valueType = (Condition.ValueType)propValueType.enumValueIndex;
                    if (valueType == Condition.ValueType.TagCount)
                    {
                        var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height / 2);
                        var tagRect = new Rect(positionValue, position.y + position.height / 2, 150 + extra_width_variable, position.height / 2);

                        EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                        EditorGUI.PropertyField(tagRect, propTag, GUIContent.none);
                    }
                    else if ((propValueType.enumValueIndex >= (int)Condition.ValueType.WorldPositionX) &&
                             (propValueType.enumValueIndex <= (int)Condition.ValueType.RelativePositionY))
                    {
                        var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height / 2);
                        var transformRect = new Rect(positionValue, position.y + position.height / 2, 150 + extra_width_variable, position.height / 2);

                        EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                        EditorGUI.PropertyField(transformRect, propTransform, GUIContent.none);
                    }
                    else if ((propValueType.enumValueIndex >= (int)Condition.ValueType.AbsoluteVelocityX) &&
                             (propValueType.enumValueIndex <= (int)Condition.ValueType.AbsoluteVelocityY))
                    {
                        var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height / 2);
                        var rbRect = new Rect(positionValue, position.y + position.height / 2, 150 + extra_width_variable, position.height / 2);

                        EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                        EditorGUI.PropertyField(rbRect, propRB, GUIContent.none);
                    }
                    else if (valueType == Condition.ValueType.Distance) 
                    {
                        var elemHeight = position.height / 2.0f;
                        if ((propTransform.objectReferenceValue == null) && (propTag.objectReferenceValue == null)) elemHeight = position.height / 3;

                        var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, elemHeight);
                        var rect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);

                        EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                        if ((propTag.objectReferenceValue) || (propTransform.objectReferenceValue == null))
                        {
                            EditorGUI.PropertyField(rect, propTag, GUIContent.none);
                            rect.y += elemHeight;
                        }
                        if ((propTransform.objectReferenceValue) || (propTag.objectReferenceValue == null))
                        {
                            EditorGUI.PropertyField(rect, propTransform, GUIContent.none);
                        }
                    }
                    else if (valueType == Condition.ValueType.Angle)
                    {
                        var elemHeight = position.height / 3.0f;
                        if ((propTransform.objectReferenceValue == null) && (propTag.objectReferenceValue == null)) elemHeight = position.height / 4;

                        var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, elemHeight);
                        var axisRect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);
                        var rect = new Rect(positionValue, axisRect.yMax, 150 + extra_width_variable, elemHeight);

                        EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                        EditorGUI.PropertyField(axisRect, propAxis, GUIContent.none);
                        if ((propTag.objectReferenceValue) || (propTransform.objectReferenceValue == null))
                        {
                            EditorGUI.PropertyField(rect, propTag, GUIContent.none);
                            rect.y += elemHeight;
                        }
                        if ((propTransform.objectReferenceValue) || (propTag.objectReferenceValue == null))
                        {
                            EditorGUI.PropertyField(rect, propTransform, GUIContent.none);
                        }
                    }
                    else if ((valueType == Condition.ValueType.Probe) || (valueType == Condition.ValueType.ProbeDistance))
                    {
                        var elemHeight = position.height / 2.0f;

                        var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, elemHeight);
                        var probeRect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);

                        EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                        EditorGUI.PropertyField(probeRect, propProbe, GUIContent.none);

                        if (valueType == Condition.ValueType.Probe) dataType = Condition.DataType.Boolean;
                    }
                }
            }
            else
            {
                var variableRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height);
                EditorGUI.PropertyField(variableRect, propVariable, GUIContent.none);
            }
        }
        else
        {
            var valueHandlerRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height);
            EditorGUI.PropertyField(valueHandlerRect, propValueHandler, GUIContent.none);
        }

        if (dataType == Condition.DataType.Number)
        {
            var propComparisonRect = new Rect(positionValue + 155 + extra_width_variable, position.y + offset_y, 110, height);
            var propValueRect = new Rect(propComparisonRect.max.x + 5, position.y, 60 + extra_width_value, height);
            var propPercentageLabelRect = new Rect(propValueRect.max.x + 10, position.y + offset_y, 20, height);
            var propPercentageRect = new Rect(propPercentageLabelRect.max.x, position.y + offset_y, 30, height);

            EditorGUI.PropertyField(propComparisonRect, propComparison, GUIContent.none);
            EditorGUI.PropertyField(propValueRect, propValue, GUIContent.none);
            EditorGUI.LabelField(propPercentageLabelRect, "%");
            EditorGUI.PropertyField(propPercentageRect, propPercentage, GUIContent.none);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var propValueHandler = property.FindPropertyRelative("valueHandler");
        var propVariable = property.FindPropertyRelative("variable");
        var systemVariable = property.FindPropertyRelative("valueType");

        if (propValueHandler.objectReferenceValue == null)
        {
            if (propVariable.objectReferenceValue == null)
            {
                if (systemVariable.enumValueIndex == 0)
                {
                    return base.GetPropertyHeight(property, label) * 3;
                }
                else if ((systemVariable.enumValueIndex >= (int)Condition.ValueType.TagCount) &&
                         (systemVariable.enumValueIndex <= (int)Condition.ValueType.AbsoluteVelocityY))
                {
                    return base.GetPropertyHeight(property, label) * 2;
                }
                else if (systemVariable.enumValueIndex == (int)Condition.ValueType.Distance)
                         
                {
                    var tagVariable = property.FindPropertyRelative("tag");
                    var transformVariable = property.FindPropertyRelative("sourceTransform");

                    if (tagVariable.objectReferenceValue != null) return base.GetPropertyHeight(property, label) * 2;
                    else if (transformVariable.objectReferenceValue != null) return base.GetPropertyHeight(property, label) * 2;
                    else return base.GetPropertyHeight(property, label) * 3;
                }
                else if (systemVariable.enumValueIndex == (int)Condition.ValueType.Angle)
                {
                    var tagVariable = property.FindPropertyRelative("tag");
                    var transformVariable = property.FindPropertyRelative("sourceTransform");

                    if (tagVariable.objectReferenceValue != null) return base.GetPropertyHeight(property, label) * 3;
                    else if (transformVariable.objectReferenceValue != null) return base.GetPropertyHeight(property, label) * 3;
                    else return base.GetPropertyHeight(property, label) * 4;
                }
                else if ((systemVariable.enumValueIndex == (int)Condition.ValueType.Probe) ||
                         (systemVariable.enumValueIndex == (int)Condition.ValueType.ProbeDistance))

                {
                    return base.GetPropertyHeight(property, label) * 2;
                }
            }
        }

        return base.GetPropertyHeight(property, label);
    }
}
