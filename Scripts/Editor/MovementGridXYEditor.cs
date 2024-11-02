using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementGridXY))]
    public class MovementGridXYEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propCooldown;
        SerializedProperty propUseRotation;
        SerializedProperty propTurnToDirection;
        SerializedProperty propMaxTurnSpeed;
        SerializedProperty propAxisToAlign;
        SerializedProperty propInputEnabled;
        SerializedProperty propInputType;
        SerializedProperty propHorizontalAxis;
        SerializedProperty propVerticalAxis;
        SerializedProperty propHorizontalButtonPositive;
        SerializedProperty propHorizontalButtonNegative;
        SerializedProperty propVerticalButtonPositive;
        SerializedProperty propVerticalButtonNegative;
        SerializedProperty propHorizontalKeyPositive;
        SerializedProperty propHorizontalKeyNegative;
        SerializedProperty propVerticalKeyPositive;
        SerializedProperty propVerticalKeyNegative;
        SerializedProperty propPushStrength;
        SerializedProperty propFlipBehaviour;
        SerializedProperty propUseAnimator;
        SerializedProperty propAnimator;
        SerializedProperty propHorizontalVelocityParameter;
        SerializedProperty propAbsoluteHorizontalVelocityParameter;
        SerializedProperty propVerticalVelocityParameter;
        SerializedProperty propAbsoluteVerticalVelocityParameter;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpeed = serializedObject.FindProperty("speed");
            propCooldown = serializedObject.FindProperty("cooldown");            
            propUseRotation = serializedObject.FindProperty("useRotation");
            propTurnToDirection = serializedObject.FindProperty("turnToDirection");
            propMaxTurnSpeed = serializedObject.FindProperty("maxTurnSpeed");
            propAxisToAlign = serializedObject.FindProperty("axisToAlign");
            propInputEnabled = serializedObject.FindProperty("inputEnabled");
            propInputType = serializedObject.FindProperty("inputType");
            propHorizontalAxis = serializedObject.FindProperty("horizontalAxis");
            propVerticalAxis = serializedObject.FindProperty("verticalAxis");
            propHorizontalButtonPositive = serializedObject.FindProperty("horizontalButtonPositive");
            propHorizontalButtonNegative = serializedObject.FindProperty("horizontalButtonNegative");
            propVerticalButtonPositive = serializedObject.FindProperty("verticalButtonPositive");
            propVerticalButtonNegative = serializedObject.FindProperty("verticalButtonNegative");
            propHorizontalKeyPositive = serializedObject.FindProperty("horizontalKeyPositive");
            propHorizontalKeyNegative = serializedObject.FindProperty("horizontalKeyNegative");
            propVerticalKeyPositive = serializedObject.FindProperty("verticalKeyPositive");
            propVerticalKeyNegative = serializedObject.FindProperty("verticalKeyNegative");
            propPushStrength = serializedObject.FindProperty("pushStrength");
            propFlipBehaviour = serializedObject.FindProperty("flipBehaviour");
            propUseAnimator = serializedObject.FindProperty("useAnimator");
            propAnimator = serializedObject.FindProperty("animator");
            propHorizontalVelocityParameter = serializedObject.FindProperty("horizontalVelocityParameter");
            propAbsoluteHorizontalVelocityParameter = serializedObject.FindProperty("absoluteHorizontalVelocityParameter");
            propVerticalVelocityParameter = serializedObject.FindProperty("verticalVelocityParameter");
            propAbsoluteVerticalVelocityParameter = serializedObject.FindProperty("absoluteVerticalVelocityParameter");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                DefaultMovementEditor();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "Maximum movement speed in world units (pixels)/second"));
                EditorGUILayout.PropertyField(propCooldown, new GUIContent("Cooldown", "Time between grid movements"));

                EditorGUILayout.PropertyField(propUseRotation, new GUIContent("Use Rotation?", "If true, the X and Y speed is relative to the rotation of the object.\nThis means that if the object is turned they refer to the right and up of the object, and not the absolute screen coordinates."));
                if (!propUseRotation.boolValue)
                {
                    EditorGUILayout.PropertyField(propTurnToDirection, new GUIContent("Turn To Movement Direction?", "If active, the object will turn towards the movement direction."));
                    if (propTurnToDirection.boolValue)
                    {
                        EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis to align", "Is the object pointing right or up?"));
                        EditorGUILayout.PropertyField(propMaxTurnSpeed, new GUIContent("Max turn speed", "What's the maximum rotation speed (degrees/second)?"));
                    }
                }

                EditorGUILayout.PropertyField(propInputEnabled, new GUIContent("Use Input?", "Is the object controlled by the player?"));
                if (propInputEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type", "What's the input type?\nAxis: Use two axis to move\nButton: Use four keys to move\nKey: Use four keys to move"));

                    var inputType = (MovementXY.InputType)propInputType.intValue;

                    switch (inputType)
                    {
                        case MovementXY.InputType.Axis:
                            EditorGUILayout.PropertyField(propHorizontalAxis, new GUIContent("Horizontal Axis", "Horizontal axis"));
                            EditorGUILayout.PropertyField(propVerticalAxis, new GUIContent("Vertical Axis", "Vertical axis"));
                            break;
                        case MovementXY.InputType.Button:
                            EditorGUILayout.PropertyField(propHorizontalButtonPositive, new GUIContent("Horizontal Positive Button", "Right button"));
                            EditorGUILayout.PropertyField(propHorizontalButtonNegative, new GUIContent("Horizontal Negative Button", "Left button"));
                            EditorGUILayout.PropertyField(propVerticalButtonPositive, new GUIContent("Vertical Positive Button", "Up button"));
                            EditorGUILayout.PropertyField(propVerticalButtonNegative, new GUIContent("Vertical Negative Button", "Down button"));
                            break;
                        case MovementXY.InputType.Key:
                            EditorGUILayout.PropertyField(propHorizontalKeyPositive, new GUIContent("Horizontal Positive Key", "Right key"));
                            EditorGUILayout.PropertyField(propHorizontalKeyNegative, new GUIContent("Horizontal Negative Key", "Left key"));
                            EditorGUILayout.PropertyField(propVerticalKeyPositive, new GUIContent("Vertical Positive Key", "Up key"));
                            EditorGUILayout.PropertyField(propVerticalKeyNegative, new GUIContent("Vertical Negative Key", "Down key"));
                            break;
                        default:
                            break;
                    }
                }

                EditorGUILayout.PropertyField(propPushStrength, new GUIContent("Push Strength", "This is the strength which is used to push other objects.\nThis is matched with the mass of the objects, and can be accumulated (i.e. you need a strength of 2 to push two objects with 1 mass at the same time).\nA strenght of zero disables the push system."));

                // Separator
                Rect separatorRect = GUILayoutUtility.GetLastRect();
                separatorRect.yMin = separatorRect.yMax + 5;
                separatorRect.height = 5.0f;
                EditorGUI.DrawRect(separatorRect, GUIUtils.ColorFromHex("#ff6060"));
                EditorGUILayout.Space(separatorRect.height + 5);

                EditorGUILayout.PropertyField(propFlipBehaviour, new GUIContent("Flip Behaviour", "What to do visually when we turn?\nVelocity Flips Sprite: When horizontal velocity is negative (moving left), the sprite renderer is flipped horizontal\nVelocity Inverts Scale: When horizontal velocity is negative (moving left), the horizontal scale is negated\nInput Flips Sprite: When players presses left, the sprite renderer is flipped horizontal\nInput Inverts Scale: When player presses left, the horizontal scale is negated\nVelocity Rotate Sprite: When horizontal velocity is negative (moving left), the object is rotated 180 degrees around the Y axis.\nInput Rotate Sprite: When the player presses left, the object is rotated 180 degrees around the Y axis.\nScaling doesn't affect the internal object's direction, while rotating does."));
                EditorGUILayout.PropertyField(propUseAnimator, new GUIContent("Use Animator", "Should we drive an animator with this movement controller?"));
                if (propUseAnimator.boolValue)
                {
                    EditorGUILayout.PropertyField(propAnimator, new GUIContent("Animator", "What animator to use?"));
                    EditorGUILayout.PropertyField(propHorizontalVelocityParameter, new GUIContent("Horizontal Velocity Parameter", "What is the parameter to set to the horizontal velocity?"));
                    EditorGUILayout.PropertyField(propAbsoluteHorizontalVelocityParameter, new GUIContent("Absolute Horizontal Velocity Parameter", "What is the parameter to set to the absolute horizontal velocity?"));
                    EditorGUILayout.PropertyField(propVerticalVelocityParameter, new GUIContent("Vertical Velocity Parameter", "What is the parameter to set to the vertical velocity?"));
                    EditorGUILayout.PropertyField(propAbsoluteVerticalVelocityParameter, new GUIContent("Absolute Vertical Velocity Parameter", "What is the parameter to set to the absolute horizontal velocity?"));
                }

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                StdEditor(false);
            }
        }

    }
}