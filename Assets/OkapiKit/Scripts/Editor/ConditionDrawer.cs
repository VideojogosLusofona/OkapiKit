using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
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
            var propValueType = property.FindPropertyRelative(nameof(Condition.valueType));
            var propNegate = property.FindPropertyRelative(nameof(Condition.negate));
            var propValueHandler = property.FindPropertyRelative(nameof(Condition.valueHandler));
            var propVariable = property.FindPropertyRelative(nameof(Condition.variable));
            var propTag = property.FindPropertyRelative(nameof(Condition.tag));
            var propTagRangeEnabled = property.FindPropertyRelative(nameof(Condition.tagCountRangeEnabled));
            var propTagRange = property.FindPropertyRelative(nameof(Condition.tagCountRange));
            var propTransform = property.FindPropertyRelative(nameof(Condition.sourceTransform));
            var propRB = property.FindPropertyRelative(nameof(Condition.rigidBody));
            var propAxis = property.FindPropertyRelative(nameof(Condition.axis));
            var propProbe = property.FindPropertyRelative(nameof(Condition.probe));
            var propMovementPlatformer = property.FindPropertyRelative(nameof(Condition.movementPlatformer));

            var propComparison = property.FindPropertyRelative(nameof(Condition.comparison));
            var propValue = property.FindPropertyRelative(nameof(Condition.value));
            var propComparisonValueHandler = property.FindPropertyRelative(nameof(Condition.comparisonValueHandler));
            var propComparisonVariable = property.FindPropertyRelative(nameof(Condition.comparisonVariable));
            var propPercentage = property.FindPropertyRelative(nameof(Condition.percentageCompare));
            var propTile = property.FindPropertyRelative(nameof(Condition.tile));
            var propTileSet = property.FindPropertyRelative(nameof(Condition.tileSet));

            float positionValue = position.x + 50;

            EditorGUI.LabelField(new Rect(position.x, position.y, 50, position.height), new GUIContent("Not", "Negate this condition"));
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
                    if (propValueType.intValue == 0)
                    {
                        // Calculate rects
                        var systemRect = new Rect(positionValue, position.y + position.height / 3, 150 + extra_width_variable, position.height / 3);
                        var valueHandlerRect = new Rect(positionValue, position.y + 2 * position.height / 3, 150 + extra_width_variable, position.height / 3);
                        var variableRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height / 3);

                        // Draw fields - pass GUIContent.none to each so they are drawn without labels
                        EditorGUI.PropertyField(systemRect, propValueType, new GUIContent("", "Function"));
                        EditorGUI.PropertyField(valueHandlerRect, propValueHandler, GUIContent.none);
                        EditorGUI.PropertyField(variableRect, propVariable, GUIContent.none);

                        offset_y = position.height / 4;
                    }
                    else
                    {
                        Condition.ValueType valueType = (Condition.ValueType)propValueType.intValue;
                        if (valueType == Condition.ValueType.TagCount)
                        {
                            float heightPerElement = (position.height / 3) - 2.0f;

                            var valueTypeRect = new Rect(positionValue, GetLineY(position.y, heightPerElement, 0), 150 + extra_width_variable, heightPerElement);
                            var tagRect = new Rect(positionValue, GetLineY(position.y, heightPerElement, 1), 150 + extra_width_variable, heightPerElement);
                            var tagRangeEnableLabelRect = new Rect(positionValue, GetLineY(position.y, heightPerElement, 2), 80, heightPerElement);
                            var tagRangeEnableRect = new Rect(positionValue + 85, GetLineY(position.y, heightPerElement, 2), 20, heightPerElement);

                            EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                            EditorGUI.PropertyField(tagRect, propTag, GUIContent.none);
                            EditorGUI.LabelField(tagRangeEnableLabelRect, new GUIContent("Limit Range?", "Should the search for this tag be limited to a range near this object?"));
                            EditorGUI.PropertyField(tagRangeEnableRect, propTagRangeEnabled, GUIContent.none);
                            if (propTagRangeEnabled.boolValue)
                            {
                                var tagRangeRect = new Rect(positionValue + 110, GetLineY(position.y, heightPerElement, 2), 40 + extra_width_variable, heightPerElement);

                                EditorGUI.PropertyField(tagRangeRect, propTagRange, GUIContent.none);
                            }
                        }
                        else if ((propValueType.intValue >= (int)Condition.ValueType.WorldPositionX) &&
                                 (propValueType.intValue <= (int)Condition.ValueType.LocalPositionY))
                        {
                            var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, position.height / 2);
                            var transformRect = new Rect(positionValue, position.y + position.height / 2, 150 + extra_width_variable, position.height / 2);

                            EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                            EditorGUI.PropertyField(transformRect, propTransform, GUIContent.none);
                        }
                        else if ((valueType == Condition.ValueType.AbsoluteVelocityX) ||
                                 (valueType == Condition.ValueType.AbsoluteVelocityY) ||
                                 (valueType == Condition.ValueType.VelocityX) ||
                                 (valueType == Condition.ValueType.VelocityY))
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
                            float lines = 2.0f;
                            if ((propProbe.objectReferenceValue == null) &&  (propTag.objectReferenceValue == null)) lines = 3.0f;
                            
                            var elemHeight = position.height / lines;

                            var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, elemHeight);
                            var probeRect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);
                            var probeTagRect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);

                            EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                            if (propProbe.objectReferenceValue == null)
                            {
                                if (propTag.objectReferenceValue == null)
                                {
                                    probeTagRect.y = probeRect.yMax;
                                    EditorGUI.PropertyField(probeRect, propProbe, GUIContent.none);
                                    EditorGUI.PropertyField(probeTagRect, propTag, GUIContent.none);
                                }
                                else
                                {
                                    EditorGUI.PropertyField(probeTagRect, propTag, GUIContent.none);
                                }
                            }
                            else
                            {
                                EditorGUI.PropertyField(probeRect, propProbe, GUIContent.none);

                            }

                            if (valueType == Condition.ValueType.Probe) dataType = Condition.DataType.Boolean;
                        }
                        else if (valueType == Condition.ValueType.IsGrounded)
                        {
                            var elemHeight = position.height / 2.0f;

                            var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, elemHeight);
                            var movementRect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);

                            EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                            EditorGUI.PropertyField(movementRect, propMovementPlatformer, GUIContent.none);

                            dataType = Condition.DataType.Boolean;
                        }
                        else if (valueType == Condition.ValueType.IsGliding)
                        {
                            var elemHeight = position.height / 2.0f;

                            var valueTypeRect = new Rect(positionValue, position.y, 150 + extra_width_variable, elemHeight);
                            var movementRect = new Rect(positionValue, position.y + elemHeight, 150 + extra_width_variable, elemHeight);

                            EditorGUI.PropertyField(valueTypeRect, propValueType, GUIContent.none);
                            EditorGUI.PropertyField(movementRect, propMovementPlatformer, GUIContent.none);

                            dataType = Condition.DataType.Boolean;
                        }
                        else if ((valueType == Condition.ValueType.OnTile) ||
                                 (valueType == Condition.ValueType.OnTileSet))
                        {
                            var elemHeight = position.height / 3.0f;
                            if ((propTransform.objectReferenceValue == null) && (propTag.objectReferenceValue == null)) elemHeight = position.height / 4;

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
                                rect.y += elemHeight;
                            }

                            if (valueType == Condition.ValueType.OnTile)
                                EditorGUI.PropertyField(rect, propTile, GUIContent.none);
                            else if (valueType == Condition.ValueType.OnTileSet)
                                EditorGUI.PropertyField(rect, propTileSet, GUIContent.none);

                            dataType = Condition.DataType.Boolean;
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

                if (propComparisonValueHandler.objectReferenceValue != null)
                {
                    EditorGUI.PropertyField(propValueRect, propComparisonValueHandler, GUIContent.none);
                }
                else if (propComparisonVariable.objectReferenceValue != null)
                {
                    EditorGUI.PropertyField(propValueRect, propComparisonVariable, GUIContent.none);
                }
                else
                {
                    var r1 = propValueRect;
                    r1.height = r1.height / 3;
                    var r2 = r1;
                    r2.y = r1.yMax;
                    var r3 = r1;
                    r3.y = r2.yMax;

                    EditorGUI.PropertyField(r1, propComparisonValueHandler, GUIContent.none);
                    EditorGUI.PropertyField(r2, propComparisonVariable, GUIContent.none);
                    EditorGUI.PropertyField(r3, propValue, GUIContent.none);
                }

                EditorGUI.LabelField(propPercentageLabelRect, "%");
                EditorGUI.PropertyField(propPercentageRect, propPercentage, GUIContent.none);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public float GetLineY(float basePosY, float heightPerElement, int lineY)
        {
            return basePosY + lineY * (heightPerElement + 2);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propValueHandler = property.FindPropertyRelative(nameof(Condition.valueHandler));
            var propVariable = property.FindPropertyRelative(nameof(Condition.variable));
            var systemVariable = property.FindPropertyRelative(nameof(Condition.valueType));
            var baseHeight = base.GetPropertyHeight(property, label) + 2;

            var propComparisonValueHandler = property.FindPropertyRelative(nameof(Condition.comparisonValueHandler));
            var propComparisonVariable = property.FindPropertyRelative(nameof(Condition.comparisonVariable));

            float height = baseHeight;

            var condType = (Condition.ValueType)systemVariable.intValue;

            if (propValueHandler.objectReferenceValue == null)
            {
                if (propVariable.objectReferenceValue == null)
                {
                    if (systemVariable.intValue == 0)
                    {
                        height = baseHeight * 3;
                    }
                    else if (systemVariable.intValue == (int)Condition.ValueType.TagCount)
                    {
                        height = baseHeight * 3;
                    }
                    else if ((condType == Condition.ValueType.AbsoluteVelocityX) ||
                             (condType == Condition.ValueType.AbsoluteVelocityY) ||
                             (condType == Condition.ValueType.VelocityX) ||
                             (condType == Condition.ValueType.VelocityY))
                    {
                        height = baseHeight * 2;
                    }
                    else if (condType == Condition.ValueType.Distance)
                    {
                        var tagVariable = property.FindPropertyRelative(nameof(Condition.tag));
                        var transformVariable = property.FindPropertyRelative(nameof(Condition.sourceTransform));

                        if (tagVariable.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 2;
                        else if (transformVariable.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 2;
                        else height = baseHeight * 3;
                    }
                    else if (condType == Condition.ValueType.Angle)
                    {
                        var tagVariable = property.FindPropertyRelative(nameof(Condition.tag));
                        var transformVariable = property.FindPropertyRelative(nameof(Condition.sourceTransform));

                        if (tagVariable.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 3;
                        else if (transformVariable.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 3;
                        else height = baseHeight * 4;
                    }
                    else if ((condType == Condition.ValueType.Probe) ||
                             (condType == Condition.ValueType.ProbeDistance))
                    {
                        var propTag = property.FindPropertyRelative(nameof(Condition.tag));
                        var propProbe = property.FindPropertyRelative(nameof(Condition.probe));

                        if (propTag.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 2;
                        else if (propProbe.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 2;
                        else height = baseHeight * 3;
                    }
                    else if ((condType == Condition.ValueType.IsGrounded) ||
                             (condType == Condition.ValueType.IsGliding))
                    {
                        height = baseHeight * 2;
                    }
                    else if ((condType == Condition.ValueType.OnTile) ||
                             (condType == Condition.ValueType.OnTileSet))
                    {
                        var tagVariable = property.FindPropertyRelative(nameof(Condition.tag));
                        var transformVariable = property.FindPropertyRelative(nameof(Condition.sourceTransform));

                        if (tagVariable.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 3;
                        else if (transformVariable.objectReferenceValue != null) height = base.GetPropertyHeight(property, label) * 3;
                        else height = baseHeight * 4;
                    }
                }
            }

            if ((condType == Condition.ValueType.Probe) ||
                (condType == Condition.ValueType.IsGrounded) ||
                (condType == Condition.ValueType.IsGliding) ||
                (condType == Condition.ValueType.OnTile) ||
                (condType == Condition.ValueType.OnTileSet))
            {
                // No need for a comparison value
            }
            else
            {
                if (propComparisonValueHandler.objectReferenceValue == null)
                {
                    if (propComparisonVariable.objectReferenceValue == null)
                    {
                        height = Mathf.Max(height, baseHeight * 3);
                    }
                }
            }

            return height;
        }

        internal static void OnSceneGUI(SerializedObject serializedObject, SerializedProperty conditionElement)
        {
            // Get type
            var propValueType = conditionElement.FindPropertyRelative(nameof(Condition.valueType));
            var type = (Condition.ValueType)propValueType.intValue;

            if (type == Condition.ValueType.TagCount)
            {
                var propTagRangeEnabled = conditionElement.FindPropertyRelative(nameof(Condition.tagCountRangeEnabled));
                var propTagRange = conditionElement.FindPropertyRelative(nameof(Condition.tagCountRange));

                if (propTagRangeEnabled.boolValue)
                {
                    Handles.color = new Color(0.1f, 1.0f, 1.0f, 0.25f);

                    OkapiElement okapiElement = serializedObject.targetObject as OkapiElement;
                    if (okapiElement)
                    {
                        Condition condition = (Condition)conditionElement.boxedValue;
                        string conditionText = condition.GetRawDescription(okapiElement.gameObject);

                        // Draw circle
                        Handles.DrawWireArc(okapiElement.transform.position, Vector3.forward, Vector3.up, 360, propTagRange.floatValue);

                        Action action = serializedObject.targetObject as Action;
                        if (action != null)
                        {
                            conditionText = action.explanation;
                        }

                        var labelStyle = GUIUtils.GetLabelStyle("#00FFFF40");
                        Handles.Label(okapiElement.transform.position + Vector3.right * (propTagRange.floatValue + 10.0f), conditionText, labelStyle);
                    }
                }
            }
        }
    }
}