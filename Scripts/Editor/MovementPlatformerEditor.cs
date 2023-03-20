using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static MovementPlatformer;

[CustomEditor(typeof(MovementPlatformer))]
public class MovementPlatformerEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propHorizontalInputType;
    SerializedProperty propHorizontalAxis;
    SerializedProperty propHorizontalButtonPositive;
    SerializedProperty propHorizontalButtonNegative;
    SerializedProperty propHorizontalKeyPositive;
    SerializedProperty propHorizontalKeyNegative;
    SerializedProperty propGravityScale;
    SerializedProperty propUseTerminalVelocity;
    SerializedProperty propTerminalVelocity;
    SerializedProperty propCoyoteTime;
    SerializedProperty propJumpBehaviour;
    SerializedProperty propMaxJumpCount;
    SerializedProperty propJumpBufferingTime;
    SerializedProperty propJumpHoldMaxTime;
    SerializedProperty propJumpInputType;
    SerializedProperty propJumpAxis;
    SerializedProperty propJumpButton;
    SerializedProperty propJumpKey;
    SerializedProperty propEnableAirControl;
    SerializedProperty propAirCollider;
    SerializedProperty propGroundCollider;
    SerializedProperty propGlideBehaviour;
    SerializedProperty propGlideMaxTime;
    SerializedProperty propMaxGlideSpeed;
    SerializedProperty propGlideInputType;
    SerializedProperty propGlideAxis;
    SerializedProperty propGlideButton;
    SerializedProperty propGlideKey;
    SerializedProperty propGroundCheckCollider;
    SerializedProperty propGroundLayerMask;
    SerializedProperty propFlipBehaviour;
    SerializedProperty propUseAnimator;
    SerializedProperty propAnimator;
    SerializedProperty propHorizontalVelocityParameter;
    SerializedProperty propAbsoluteHorizontalVelocityParameter;
    SerializedProperty propVerticalVelocityParameter;
    SerializedProperty propAbsoluteVerticalVelocityParameter;
    SerializedProperty propIsGroundedParameter;
    SerializedProperty propIsGlidingParameter;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");

        propHorizontalInputType = serializedObject.FindProperty("horizontalInputType");
        propHorizontalAxis = serializedObject.FindProperty("horizontalAxis");
        propHorizontalButtonPositive = serializedObject.FindProperty("horizontalButtonPositive");
        propHorizontalButtonNegative = serializedObject.FindProperty("horizontalButtonNegative");
        propHorizontalKeyPositive = serializedObject.FindProperty("horizontalKeyPositive");
        propHorizontalKeyNegative = serializedObject.FindProperty("horizontalKeyNegative");
        propGravityScale = serializedObject.FindProperty("gravityScale");
        propUseTerminalVelocity = serializedObject.FindProperty("useTerminalVelocity");
        propTerminalVelocity = serializedObject.FindProperty("terminalVelocity");
        propCoyoteTime = serializedObject.FindProperty("coyoteTime");
        propJumpBehaviour = serializedObject.FindProperty("jumpBehaviour");
        propMaxJumpCount = serializedObject.FindProperty("maxJumpCount");
        propJumpBufferingTime = serializedObject.FindProperty("jumpBufferingTime");
        propJumpHoldMaxTime = serializedObject.FindProperty("jumpHoldMaxTime");
        propJumpInputType = serializedObject.FindProperty("jumpInputType");
        propJumpAxis = serializedObject.FindProperty("jumpAxis");
        propJumpButton = serializedObject.FindProperty("jumpButton");
        propJumpKey = serializedObject.FindProperty("jumpKey");
        propEnableAirControl = serializedObject.FindProperty("enableAirControl");
        propAirCollider = serializedObject.FindProperty("airCollider");
        propGroundCollider = serializedObject.FindProperty("groundCollider");
        propGlideBehaviour = serializedObject.FindProperty("glideBehaviour");
        propGlideMaxTime = serializedObject.FindProperty("glideMaxTime");
        propMaxGlideSpeed = serializedObject.FindProperty("maxGlideSpeed");
        propGlideInputType = serializedObject.FindProperty("glideInputType");
        propGlideAxis = serializedObject.FindProperty("glideAxis");
        propGlideButton = serializedObject.FindProperty("glideButton");
        propGlideKey = serializedObject.FindProperty("glideKey");
        propGroundCheckCollider = serializedObject.FindProperty("groundCheckCollider");
        propGroundLayerMask = serializedObject.FindProperty("groundLayerMask");
        propFlipBehaviour = serializedObject.FindProperty("flipBehaviour");
        propUseAnimator = serializedObject.FindProperty("useAnimator");
        propAnimator = serializedObject.FindProperty("animator");
        propHorizontalVelocityParameter= serializedObject.FindProperty("horizontalVelocityParameter");
        propAbsoluteHorizontalVelocityParameter = serializedObject.FindProperty("absoluteHorizontalVelocityParameter");
        propVerticalVelocityParameter = serializedObject.FindProperty("verticalVelocityParameter");
        propAbsoluteVerticalVelocityParameter = serializedObject.FindProperty("absoluteVerticalVelocityParameter");
        propIsGroundedParameter = serializedObject.FindProperty("isGroundedParameter");
        propIsGlidingParameter = serializedObject.FindProperty("isGlidingParameter");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));

            EditorGUILayout.PropertyField(propHorizontalInputType, new GUIContent("Horizontal Input Type"));

            var inputType = (InputType)propHorizontalInputType.enumValueIndex;

            switch (inputType)
            {
                case InputType.Axis:
                    EditorGUILayout.PropertyField(propHorizontalAxis, new GUIContent("Horizontal Axis"));
                    break;
                case InputType.Button:
                    EditorGUILayout.PropertyField(propHorizontalButtonPositive, new GUIContent("Horizontal Positive Button"));
                    EditorGUILayout.PropertyField(propHorizontalButtonNegative, new GUIContent("Horizontal Negative Button"));
                    break;
                case InputType.Key:
                    EditorGUILayout.PropertyField(propHorizontalKeyPositive, new GUIContent("Horizontal Positive Key"));
                    EditorGUILayout.PropertyField(propHorizontalKeyNegative, new GUIContent("Horizontal Negative Key"));
                    break;
                default:
                    break;
            }

            EditorGUILayout.PropertyField(propGroundCheckCollider, new GUIContent("Ground Check Collider"));
            EditorGUILayout.PropertyField(propGroundLayerMask, new GUIContent("Ground Layer Mask"));
            EditorGUILayout.PropertyField(propGravityScale, new GUIContent("Gravity Scale"));
            EditorGUILayout.PropertyField(propUseTerminalVelocity, new GUIContent("Use Terminal Velocity"));
            if (propUseTerminalVelocity.boolValue)
            {
                EditorGUILayout.PropertyField(propTerminalVelocity, new GUIContent("Terminal Velocity"));
            }
            EditorGUILayout.PropertyField(propCoyoteTime, new GUIContent("Coyote Time"));
            EditorGUILayout.PropertyField(propEnableAirControl, new GUIContent("Air Control"));
            EditorGUILayout.PropertyField(propAirCollider, new GUIContent("Air Collider"));
            EditorGUILayout.PropertyField(propGroundCollider, new GUIContent("Ground Collider"));

            EditorGUILayout.PropertyField(propJumpBehaviour, new GUIContent("Jump Type"));
            if (propJumpBehaviour.enumValueIndex != (int)JumpBehaviour.None)
            {
                EditorGUILayout.PropertyField(propJumpInputType, new GUIContent("Jump Input Type"));

                var jumpInputType = (InputType)propJumpInputType.enumValueIndex;

                switch (jumpInputType)
                {
                    case InputType.Axis:
                        EditorGUILayout.PropertyField(propJumpAxis, new GUIContent("Jump Axis"));
                        break;
                    case InputType.Button:
                        EditorGUILayout.PropertyField(propJumpButton, new GUIContent("Jump Button"));
                        break;
                    case InputType.Key:
                        EditorGUILayout.PropertyField(propJumpKey, new GUIContent("Jump Key"));
                        break;
                    default:
                        break;
                }

                EditorGUILayout.PropertyField(propMaxJumpCount, new GUIContent("Max Jump Count"));
                EditorGUILayout.PropertyField(propJumpBufferingTime, new GUIContent("Jump Buffering Time"));
                if (propJumpBehaviour.enumValueIndex == (int)JumpBehaviour.Variable)
                {
                    EditorGUILayout.PropertyField(propJumpHoldMaxTime, new GUIContent("Jump Max. Hold Time"));
                }
            }
            EditorGUILayout.PropertyField(propGlideBehaviour, new GUIContent("Glide Behaviour"));
            if (propGlideBehaviour.enumValueIndex != (int)GlideBehaviour.None)
            {
                EditorGUILayout.PropertyField(propGlideInputType, new GUIContent("Glide Input Type"));

                var glideInputType = (InputType)propGlideInputType.enumValueIndex;

                switch (glideInputType)
                {
                    case InputType.Axis:
                        EditorGUILayout.PropertyField(propGlideAxis, new GUIContent("Glide Axis"));
                        break;
                    case InputType.Button:
                        EditorGUILayout.PropertyField(propGlideButton, new GUIContent("Glide Button"));
                        break;
                    case InputType.Key:
                        EditorGUILayout.PropertyField(propGlideKey, new GUIContent("Glide Key"));
                        break;
                    default:
                        break;
                }

                if (propGlideBehaviour.enumValueIndex == (int)GlideBehaviour.Timer)
                {
                    EditorGUILayout.PropertyField(propGlideMaxTime, new GUIContent("Glide Max. Time"));
                }

                EditorGUILayout.PropertyField(propMaxGlideSpeed, new GUIContent("Max Glide Speed"));
            }

            // Separator
            Rect separatorRect = GUILayoutUtility.GetLastRect();
            separatorRect.yMin = separatorRect.yMax + 5;
            separatorRect.height = 5.0f;
            EditorGUI.DrawRect(separatorRect, GUIUtils.ColorFromHex("#ff6060"));
            EditorGUILayout.Space(separatorRect.height + 5);

            EditorGUILayout.PropertyField(propFlipBehaviour, new GUIContent("Flip Behaviour"));
            EditorGUILayout.PropertyField(propUseAnimator, new GUIContent("Use Animator"));
            if (propUseAnimator.boolValue)
            {
                EditorGUILayout.PropertyField(propHorizontalVelocityParameter, new GUIContent("Horizontal Velocity Parameter"));
                EditorGUILayout.PropertyField(propAbsoluteHorizontalVelocityParameter, new GUIContent("Absolute Horizontal Velocity Parameter"));
                EditorGUILayout.PropertyField(propVerticalVelocityParameter, new GUIContent("Vertical Velocity Parameter"));
                EditorGUILayout.PropertyField(propAbsoluteVerticalVelocityParameter, new GUIContent("Absolute Vertical Velocity Parameter"));
                EditorGUILayout.PropertyField(propIsGroundedParameter, new GUIContent("Is Grounded Parameter"));
                EditorGUILayout.PropertyField(propIsGlidingParameter, new GUIContent("Is Glidding Parameter"));
            }

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();

            StdEditor(false);
        }
    }

}
