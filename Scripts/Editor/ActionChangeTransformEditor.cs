using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeTransform))]
    public class ActionChangeTransformEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propChangeType;
        SerializedProperty propXAxis;
        SerializedProperty propPositionX;
        SerializedProperty propDeltaX;
        SerializedProperty propYAxis;
        SerializedProperty propPositionY;
        SerializedProperty propDeltaY;
        SerializedProperty propScaleWithTime;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propChangeType = serializedObject.FindProperty("changeType");
            propXAxis = serializedObject.FindProperty("xAxis");
            propPositionX = serializedObject.FindProperty("positionX");
            propDeltaX = serializedObject.FindProperty("deltaX");
            propYAxis = serializedObject.FindProperty("yAxis");
            propPositionY = serializedObject.FindProperty("positionY");
            propDeltaY = serializedObject.FindProperty("deltaY");
            propScaleWithTime = serializedObject.FindProperty("scaleWithTime"); ;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeTransform);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the target transform?"));
                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What kind of change you want to do on the transform?\nPosition: Change the position."));

                var changeType = (ActionChangeTransform.ChangeType)propChangeType.intValue;
                var axisChangeX = (ActionChangeTransform.AxisChange)propXAxis.intValue;
                var axisChangeY = (ActionChangeTransform.AxisChange)propYAxis.intValue;

                if (changeType == ActionChangeTransform.ChangeType.Position)
                {
                    EditorGUILayout.PropertyField(propXAxis, new GUIContent("X Axis", "What do you want to do with the X axis?\nNone: Don't change the X position\nAddSub: Add/Subtract an amount to the current X coordinate.\nMultiply: Multiply an amount to the current X coordinate.\nDivide: Divide current X coordinate by a certain amount.\nSet: Set the X coordinate to some value."));
                    if (axisChangeX == ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionX, new GUIContent("X Position (random value between [X..Y])", "What's the new value for the X coordinate?\nA value in the given range will be selected."));
                    }
                    else if (axisChangeX == ActionChangeTransform.AxisChange.AddSub)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to add/subtract from the X coordinate? A random value in the given range will be selected."));
                    }
                    else if (axisChangeX == ActionChangeTransform.AxisChange.Multiply)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to multiply with the X coordinate? A random value in the given range will be selected."));
                    }
                    else if (axisChangeX == ActionChangeTransform.AxisChange.Divide)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to divide from the X coordinate? A random value in the given range will be selected."));
                    }

                    EditorGUILayout.PropertyField(propYAxis, new GUIContent("Y Axis", "What do you want to do with the Y axis?\nNone: Don't change the Y position\nAddSub: Add/Subtract an amount to the current Y coordinate.\nMultiply: Multiply an amount to the current Y coordinate.\nDivide: Divide current Y coordinate by a certain amount.\nSet: Set the Y coordinate to some value."));
                    if (axisChangeY == ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionY, new GUIContent("Y Position (random value between [X..Y])", "What's the new value for the Y coordinate?\nA value in the given range will be selected."));
                    }
                    else if (axisChangeY == ActionChangeTransform.AxisChange.AddSub)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to add/subtract from the Y coordinate? A random value in the given range will be selected."));
                    }
                    else if (axisChangeY == ActionChangeTransform.AxisChange.Multiply)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to multiply with the Y coordinate? A random value in the given range will be selected."));
                    }
                    else if (axisChangeY == ActionChangeTransform.AxisChange.Divide)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to divide from the Y coordinate? A random value in the given range will be selected."));
                    }
                }
                else if (changeType == ActionChangeTransform.ChangeType.Scale)
                {
                    EditorGUILayout.PropertyField(propXAxis, new GUIContent("X Axis", "What do you want to do with the X axis?\nNone: Don't change the X scale\nAddSub: Add/Subtract an amount to the current X scale.\nMultiply: Multiply an amount to the current X scale.\nDivide: Divide current X scale by a certain amount.\nSet: Set the X scale to some value."));
                    if (axisChangeX == ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionX, new GUIContent("X scale (random value between [X..Y])", "What's the new value for the X scale?\nA value in the given range will be selected."));
                    }
                    else if (axisChangeX == ActionChangeTransform.AxisChange.AddSub)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to add/subtract from the X scale? A random value in the given range will be selected."));
                    }
                    else if (axisChangeX == ActionChangeTransform.AxisChange.Multiply)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to multiply with the X scale? A random value in the given range will be selected."));
                    }
                    else if (axisChangeX == ActionChangeTransform.AxisChange.Divide)
                    {
                        EditorGUILayout.PropertyField(propDeltaX, new GUIContent("Delta X (random value between [X..Y])", "How much to divide from the X scale? A random value in the given range will be selected."));
                    }

                    EditorGUILayout.PropertyField(propYAxis, new GUIContent("Y Axis", "What do you want to do with the Y axis?\nNone: Don't change the Y scale\nAddSub: Add/Subtract an amount to the current Y scale.\nMultiply: Multiply an amount to the current Y scale.\nDivide: Divide current Y scale by a certain amount.\nSet: Set the Y scaleto some value."));
                    if (axisChangeY == ActionChangeTransform.AxisChange.Set)
                    {
                        EditorGUILayout.PropertyField(propPositionY, new GUIContent("Y Position (random value between [X..Y])", "What's the new value for the Y scale?\nA value in the given range will be selected."));
                    }
                    else if (axisChangeY == ActionChangeTransform.AxisChange.AddSub)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to add/subtract from the Y scale? A random value in the given range will be selected."));
                    }
                    else if (axisChangeY == ActionChangeTransform.AxisChange.Multiply)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to multiply with the Y scale? A random value in the given range will be selected."));
                    }
                    else if (axisChangeY == ActionChangeTransform.AxisChange.Divide)
                    {
                        EditorGUILayout.PropertyField(propDeltaY, new GUIContent("Delta Y (random value between [X..Y])", "How much to divide from the Y scale? A random value in the given range will be selected."));
                    }
                }

                if ((axisChangeX == ActionChangeTransform.AxisChange.AddSub) ||
                    (axisChangeX == ActionChangeTransform.AxisChange.Multiply) ||
                    (axisChangeX == ActionChangeTransform.AxisChange.Divide) ||
                    (axisChangeY == ActionChangeTransform.AxisChange.AddSub) ||
                    (axisChangeY == ActionChangeTransform.AxisChange.Multiply) ||
                    (axisChangeY == ActionChangeTransform.AxisChange.Divide))
                {
                    EditorGUILayout.PropertyField(propScaleWithTime, new GUIContent("Scale with time?", "Should the variation be scaled with time? Useful when something happens over time."));
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