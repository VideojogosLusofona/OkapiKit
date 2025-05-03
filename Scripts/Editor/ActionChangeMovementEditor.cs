using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(OkapiKit.ActionChangeMovement))]
    public class ActionChangeMovementEditor : ActionEditor
    {
        SerializedProperty propMovementComponent;
        SerializedProperty propRigidBodyComponent;
        SerializedProperty propChangeType;
        SerializedProperty propOperation;
        SerializedProperty propFloatPlatformerOperation;
        SerializedProperty propPercentageValue;
        SerializedProperty propValue;
        SerializedProperty propAxis;
        SerializedProperty propUseRotation;
        SerializedProperty propUseRandom;
        SerializedProperty propStartAngle;
        SerializedProperty propEndAngle;
        SerializedProperty propSpeedRange;
        SerializedProperty propMinVelocity;
        SerializedProperty propMaxVelocity;
        SerializedProperty propClampSpeed;
        SerializedProperty propClampTo;
        SerializedProperty propIValue;

        protected override void OnEnable()
        {
            base.OnEnable();

            propMovementComponent = serializedObject.FindProperty("movementComponent");
            propRigidBodyComponent = serializedObject.FindProperty("rigidBodyComponent");
            propChangeType = serializedObject.FindProperty("changeType");
            propOperation = serializedObject.FindProperty("operation");
            propFloatPlatformerOperation = serializedObject.FindProperty("floatPlatformerOperation");
            propPercentageValue = serializedObject.FindProperty("percentageValue");
            propValue = serializedObject.FindProperty("value");
            propAxis = serializedObject.FindProperty("axis");
            propUseRotation = serializedObject.FindProperty("useRotation");
            propUseRandom = serializedObject.FindProperty("useRandom");
            propStartAngle = serializedObject.FindProperty("startAngle");
            propEndAngle = serializedObject.FindProperty("endAngle");
            propSpeedRange = serializedObject.FindProperty("speedRange");
            propMinVelocity = serializedObject.FindProperty("minVelocity");
            propMaxVelocity = serializedObject.FindProperty("maxVelocity");
            propClampSpeed = serializedObject.FindProperty("clampSpeed");
            propClampTo = serializedObject.FindProperty("clampTo");
            propIValue = serializedObject.FindProperty("iValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                EditorGUI.BeginChangeCheck();

                if (propMovementComponent.objectReferenceValue == null)
                {
                    if (propRigidBodyComponent.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propMovementComponent, new GUIContent("Movement Component", "What is the target movement component?\nYou can choose a target movement component, or a rigid body, but not both."));
                        EditorGUILayout.PropertyField(propRigidBodyComponent, new GUIContent("Rigidbody Component", "What is the target rigid body component?\nYou can choose a target movement component, or a rigid body, but not both."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propRigidBodyComponent, new GUIContent("Rigidbody Component", "What is the target rigid body component?\nYou can choose a target movement component, or a rigid body, but not both."));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propMovementComponent, new GUIContent("Movement Component", "What is the target movement component?\nYou can choose a target movement component, or a rigid body, but not both."));
                }

                EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type", "What is the type of change you want to perform on the movement?\nVelocity: change velocity of target\nGravity scale: scale factor for the gravity set in the project settings (Plaform Movement only)\nMax Jump Count: The maximum number of jumps the character can perform (Plaform Movement only)\nJump Hold Time: The time the player can press the jump button to jump higher (Platform Movement only)\nGliding Time: The time the player can hold the glide key ((Plaform Movement only)"));

                var changeType = (ActionChangeMovement.ChangeType)propChangeType.intValue;

                if (changeType == ActionChangeMovement.ChangeType.Velocity)
                {
                    EditorGUILayout.PropertyField(propOperation, new GUIContent("Operation", "What change to you want to perform to the velocity?\nSet: Select a value to set\nPercentage Modify: Selects a percentage of the current value to set the velocity\nAbsolute Modify: Selects an amount to increase/decrease the velocity."));

                    var operation = (ActionChangeMovement.VelocityOperation)propOperation.intValue;

                    if (operation == ActionChangeMovement.VelocityOperation.Set)
                    {
                        Rect rect = EditorGUILayout.BeginHorizontal();
                        rect.height = 20;
                        float totalWidth = rect.width;
                        float elemWidth = totalWidth / 2;
                        propUseRotation.boolValue = CheckBox("Use Rotation", "If active, the values are relative to the up/right of the object (i.e. it accounts for the objects rotation)", rect.x, rect.y, elemWidth, propUseRotation.boolValue);
                        propUseRandom.boolValue = CheckBox("Angular random", "If active, instead of specifying the velocity as a range of minimum and maximum velocity (in both axis), you define an angle range and a speed range.", rect.x + elemWidth, rect.y, elemWidth, propUseRandom.boolValue);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(rect.height);

                        if (propUseRandom.boolValue)
                        {
                            string localText = (propUseRandom.boolValue)?("local"):("");
                            EditorGUILayout.PropertyField(propStartAngle, new GUIContent("Start Angle", $"What's the minimum angle for the random. Angle = 0 means in the direction of the {localText} X axis"));
                            EditorGUILayout.PropertyField(propEndAngle, new GUIContent("End Angle", $"What's the maximum angle for the random. Angle = 90 means in the direction of the {localText} Y axis"));
                            EditorGUILayout.PropertyField(propSpeedRange, new GUIContent("Speed Range", "The velocity will have a speed between the value in X and the value in Y."));
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(propMinVelocity, new GUIContent("Min Velocity", "This is the lower bound of the velocity (in both axis)."));
                            EditorGUILayout.PropertyField(propMaxVelocity, new GUIContent("Max Velocity", "This is the top bound of the velocity (in both axis)."));
                        }
                    }
                    else if (operation == OkapiKit.ActionChangeMovement.VelocityOperation.PercentageModify)
                    {
                        EditorGUILayout.PropertyField(propPercentageValue, new GUIContent("Percentage Value [0 to 1]", "For how much to change the current velocity?\nIf zero, the velocity will be set to zero, if one the velocity will be unchanged, any other value will be multiplied by the current speed in both axis."));
                    }
                    else if (operation == OkapiKit.ActionChangeMovement.VelocityOperation.AxisModify)
                    {
                        EditorGUILayout.PropertyField(propValue, new GUIContent("Value", "Value range to add/subtract to the velocity.\nX is the lower bound, Y is the upper bound."));
                        EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis", "Allows you to select the axis for the value.\nFor example, if you choose 'Absolute Right', with a value of [X=10, Y=10], the velocity will be increased in the absolute horizontal direction (right) by 10 units per second.\nRelative axis are relative to the current direction of the object.\nCurrent is relative to the current velocity (so the direction of the velocity will be the same, even if the velocity changes).\nInverse current is the same, but in the opposite direction."));
                        EditorGUILayout.PropertyField(propClampSpeed, new GUIContent("Clamp Speed", "Activate this option to be able to select a minimum/maximum speed for the target."));
                        if (propClampSpeed.boolValue)
                        {
                            EditorGUILayout.PropertyField(propClampTo, new GUIContent("Clamp To", "Minimum/Maximum speed of the target object."));
                        }
                    }
                    else if (operation == OkapiKit.ActionChangeMovement.VelocityOperation.AxisSet)
                    {
                        EditorGUILayout.PropertyField(propValue, new GUIContent("Value", "Value range to set the velocity.\nX is the lower bound, Y is the upper bound."));
                        EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis", "Allows you to select the axis for the value.\nFor example, if you choose 'Absolute Right', with a value of [X=10, Y=10], the velocity will be increased in the absolute horizontal direction (right) by 10 units per second.\nRelative axis are relative to the current direction of the object.\nCurrent is relative to the current velocity (so the direction of the velocity will be the same, even if the velocity changes).\nInverse current is the same, but in the opposite direction."));
                        EditorGUILayout.PropertyField(propClampSpeed, new GUIContent("Clamp Speed", "Activate this option to be able to select a minimum/maximum speed for the target."));
                        if (propClampSpeed.boolValue)
                        {
                            EditorGUILayout.PropertyField(propClampTo, new GUIContent("Clamp To", "Minimum/Maximum speed of the target object."));
                        }
                    }
                }
                else if ((changeType == ActionChangeMovement.ChangeType.GravityScale) ||
                         (changeType == ActionChangeMovement.ChangeType.JumpHoldTime) ||
                         (changeType == ActionChangeMovement.ChangeType.GlideMaxTime))
                {
                    MovementPlatformer platMovement = null;
                    if (propMovementComponent.objectReferenceValue != null)
                    {
                        platMovement = propMovementComponent.objectReferenceValue as MovementPlatformer;
                    }
                    else
                    {
                        var action = target as Action;
                        platMovement = action.gameObject.GetComponent<MovementPlatformer>();
                    }
                    if (platMovement)
                    {
                        EditorGUILayout.PropertyField(propFloatPlatformerOperation, new GUIContent("Operation", $"Choose what operation to perform on the current {changeType}.\nSet: Set a value\nPercentage Modify: Set to a percentage of the current value."));

                        var operation = (ActionChangeMovement.FloatPlatformerPropertyOperation)propFloatPlatformerOperation.intValue;

                        if (operation == ActionChangeMovement.FloatPlatformerPropertyOperation.Set)
                        {
                            EditorGUILayout.PropertyField(propPercentageValue, new GUIContent("Value", $"Value range to set {changeType}."));
                        }
                        else if (operation == ActionChangeMovement.FloatPlatformerPropertyOperation.PercentageModify)
                        {
                            EditorGUILayout.PropertyField(propPercentageValue, new GUIContent("Percentage Value [0 to 1]", $"Percentage range to set {changeType}."));
                        }
                    }
                }
                else if (changeType == ActionChangeMovement.ChangeType.MaxJumpCount)
                {
                    MovementPlatformer platMovement = null;
                    if (propMovementComponent.objectReferenceValue != null)
                    {
                        platMovement = propMovementComponent.objectReferenceValue as MovementPlatformer;
                    }
                    else
                    {
                        var action = target as Action;
                        platMovement = action.gameObject.GetComponent<MovementPlatformer>();
                    }
                    if (platMovement)
                    {
                        EditorGUILayout.PropertyField(propIValue, new GUIContent("Value", $"Value to set {changeType}."));
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as OkapiElement).UpdateExplanation();
                }
            }
        }
    }
}