using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionModifyMovement))]
public class ActionModifyMovementEditor : ActionEditor
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
                    EditorGUILayout.PropertyField(propMovementComponent, new GUIContent("Movement Component"));
                    EditorGUILayout.PropertyField(propRigidBodyComponent, new GUIContent("Rigidbody Component"));
                }
                else
                {
                    EditorGUILayout.PropertyField(propRigidBodyComponent, new GUIContent("Rigidbody Component"));
                }
            }
            else
            {
                EditorGUILayout.PropertyField(propMovementComponent, new GUIContent("Movement Component"));
            }

            EditorGUILayout.PropertyField(propChangeType, new GUIContent("Change Type"));
            if (propChangeType.enumValueIndex == (int)ActionModifyMovement.ChangeType.Velocity)
            {
                EditorGUILayout.PropertyField(propOperation, new GUIContent("Operation"));

                var operation = (ActionModifyMovement.VelocityOperation)propOperation.enumValueIndex;

                if (operation == ActionModifyMovement.VelocityOperation.Set)
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    rect.height = 20;
                    float totalWidth = rect.width;
                    float elemWidth = totalWidth / 2;
                    propUseRotation.boolValue = CheckBox("Use Rotation", rect.x, rect.y, elemWidth, propUseRotation.boolValue);
                    propUseRandom.boolValue = CheckBox("Angular random", rect.x + elemWidth, rect.y, elemWidth, propUseRandom.boolValue);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(rect.height);

                    if (propUseRandom.boolValue)
                    {
                        EditorGUILayout.PropertyField(propStartAngle, new GUIContent("Start Angle"));
                        EditorGUILayout.PropertyField(propEndAngle, new GUIContent("End Angle"));
                        EditorGUILayout.PropertyField(propSpeedRange, new GUIContent("Speed Range"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propMinVelocity, new GUIContent("Min Velocity"));
                        EditorGUILayout.PropertyField(propMaxVelocity, new GUIContent("Max Velocity"));
                    }
                }
                else if (operation == ActionModifyMovement.VelocityOperation.PercentageModify)
                {
                    EditorGUILayout.PropertyField(propPercentageValue, new GUIContent("Percentage Value [0 to 1]"));
                }
                else if (operation == ActionModifyMovement.VelocityOperation.AbsoluteModify)
                {
                    EditorGUILayout.PropertyField(propValue, new GUIContent("Value"));
                    EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis"));
                }

                if ((operation == ActionModifyMovement.VelocityOperation.AbsoluteModify) ||
                    (operation == ActionModifyMovement.VelocityOperation.AbsoluteModify))
                {
                    EditorGUILayout.PropertyField(propClampSpeed, new GUIContent("Clamp Speed"));
                    if (propClampSpeed.boolValue)
                    {
                        EditorGUILayout.PropertyField(propClampTo, new GUIContent("Clamp To"));
                    }
                }
            }
            else if ((propChangeType.enumValueIndex == (int)ActionModifyMovement.ChangeType.GravityScale) ||
                     (propChangeType.enumValueIndex == (int)ActionModifyMovement.ChangeType.JumpHoldTime) ||
                     (propChangeType.enumValueIndex == (int)ActionModifyMovement.ChangeType.GlideMaxTime))
            {
                MovementPlatformer platMovement = null;
                if (propMovementComponent.objectReferenceValue != null)
                {
                    platMovement = propMovementComponent.objectReferenceValue as MovementPlatformer;
                }
                if (platMovement)
                {
                    EditorGUILayout.PropertyField(propFloatPlatformerOperation, new GUIContent("Operation"));

                    var operation = (ActionModifyMovement.FloatPlatformerPropertyOperation)propFloatPlatformerOperation.enumValueIndex;

                    if (operation == ActionModifyMovement.FloatPlatformerPropertyOperation.Set)
                    {
                        EditorGUILayout.PropertyField(propPercentageValue, new GUIContent("Value"));
                    }
                    else if (operation == ActionModifyMovement.FloatPlatformerPropertyOperation.PercentageModify)
                    {
                        EditorGUILayout.PropertyField(propPercentageValue, new GUIContent("Percentage Value [0 to 1]"));
                    }                
                }
            }
            else if (propChangeType.enumValueIndex == (int)ActionModifyMovement.ChangeType.MaxJumpCount)
            {
                MovementPlatformer platMovement = null;
                if (propMovementComponent.objectReferenceValue != null)
                {
                    platMovement = propMovementComponent.objectReferenceValue as MovementPlatformer;
                }
                if (platMovement)
                {
                    EditorGUILayout.PropertyField(propIValue, new GUIContent("Value"));
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
