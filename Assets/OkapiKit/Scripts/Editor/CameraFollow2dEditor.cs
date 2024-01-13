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

                EditorGUILayout.PropertyField(propMode, new GUIContent("Mode", "Camera follow mode\nSimple Feedback Loop: Camera will try to get as close of target, fast when it's far away and getting slower as it gets closer.\nBox: Camera trap, camera will always keep the target object inside the defined box."));

                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetObject.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "What's the tag of the object(s) to follow?\nNote that you can follow objects by tag or by linking, but not both at the same time."));
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the object to follow?\nNote that you can follow objects by tag or by linking, but not both at the same time."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the object to follow?\nNote that you can follow objects by tag or by linking, but not both at the same time."));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "What's the tag of the object(s) to follow?\nNote that you can follow objects by tag or by linking, but not both at the same time."));
                    EditorGUILayout.PropertyField(propTagMode, new GUIContent("Tag Mode", "If there are multiple objects with this tag, what to do?\nClosest: Follow the closest to the camera\nFurthest: Follow the furthest away\nAverage: Follow the average position of the objects."));
                    if (propTagMode.enumValueIndex == (int)CameraFollow2d.TagMode.Average)
                    {
                        EditorGUILayout.PropertyField(propAllowZoom, new GUIContent("Allow Zoom", "Should the camera zoom in/out to account for multiple objects being tracked?"));
                        if (propAllowZoom.boolValue)
                        {
                            EditorGUILayout.PropertyField(propZoomMargin, new GUIContent("Zoom Margin", "Margin to add to the zoom so that objects aren't just at the edge of the camera.\nIf this is 1, the objects will be right at the edge, 1.1 leaves a 10% margin, and so forth."));
                            EditorGUILayout.PropertyField(propMinMaxSize, new GUIContent("Min/Max Size", "Minimum/maximum ortographic size of the camera on the zoom."));
                        }
                    }
                }

                if (propMode.enumValueIndex == (int)CameraFollow2d.Mode.SimpleFeedbackLoop)
                {
                    EditorGUILayout.PropertyField(propFollowSpeed, new GUIContent("Follow Speed", "What's the speed of the camera while following, expressed as percentage per frame.\nIf 1, camera will be locked to the target.\nUsually a value like 0.05 (5% per frame) works fine."));
                }
                else if (propMode.enumValueIndex == (int)CameraFollow2d.Mode.Box)
                {
                    EditorGUILayout.PropertyField(propRect, new GUIContent("Box", "Camera trap position/size, you can see it in magenta on the scene view."));
                }
                EditorGUILayout.PropertyField(propCameraLimits, new GUIContent("Camera Limits", "Box collider that defines the limits of the camera movement. If target leaves this area, camera can't track it. Leave it empty for no limits (unadvisable)."));

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "Text description of this component, if you want to leave notes for yourself or others."));

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