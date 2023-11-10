using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(CameraFollow2d))]
    public class CameraFollow2dEditor : OkapiBaseEditor
    {
        SerializedProperty propMode;
        SerializedProperty propTargetTag;
        SerializedProperty propTagMode;
        SerializedProperty propAllowZoom;
        SerializedProperty propZoomMargin;
        SerializedProperty propMinMaxSize;
        SerializedProperty propTargetObject;
        SerializedProperty propFollowSpeed;
        SerializedProperty propRect;
        SerializedProperty propCameraLimits;

        protected override void OnEnable()
        {
            base.OnEnable();

            propMode = serializedObject.FindProperty("mode");
            propTargetTag = serializedObject.FindProperty("targetTag");
            propTagMode = serializedObject.FindProperty("tagMode");
            propAllowZoom = serializedObject.FindProperty("allowZoom");
            propZoomMargin = serializedObject.FindProperty("zoomMargin");
            propMinMaxSize = serializedObject.FindProperty("minMaxSize");
            propTargetObject = serializedObject.FindProperty("targetObject");
            propFollowSpeed = serializedObject.FindProperty("followSpeed");
            propRect = serializedObject.FindProperty("rect");
            propCameraLimits = serializedObject.FindProperty("cameraLimits");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propMode, new GUIContent("Mode"));

                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetObject.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                    EditorGUILayout.PropertyField(propTagMode, new GUIContent("Tag Mode"));
                    if (propTagMode.enumValueIndex == (int)CameraFollow2d.TagMode.Average)
                    {
                        EditorGUILayout.PropertyField(propAllowZoom, new GUIContent("Allow Zoom"));
                        if (propAllowZoom.boolValue)
                        {
                            EditorGUILayout.PropertyField(propZoomMargin, new GUIContent("Zoom Margin"));
                            EditorGUILayout.PropertyField(propMinMaxSize, new GUIContent("Min/Max Size"));
                        }
                    }
                }

                if (propMode.enumValueIndex == (int)CameraFollow2d.Mode.SimpleFeedbackLoop)
                {
                    EditorGUILayout.PropertyField(propFollowSpeed, new GUIContent("Follow Speed"));
                }
                else if (propMode.enumValueIndex == (int)CameraFollow2d.Mode.Box)
                {
                    EditorGUILayout.PropertyField(propRect, new GUIContent("Box"));
                    EditorGUILayout.PropertyField(propCameraLimits, new GUIContent("Camera Limits"));
                }

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                StdEditor(false);
            }
        }

        protected void StdEditor(bool useOriginalEditor = true)
        {
            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
            }

        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override string GetTitle()
        {
            return "Camera Follow";
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Movement");

            return varTexture;
        }


        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));

    }
}