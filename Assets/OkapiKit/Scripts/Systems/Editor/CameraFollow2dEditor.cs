using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraFollow2d))]
public class CameraFollow2dEditor : Editor
{
    SerializedProperty propShowInfo;
    SerializedProperty propDescription;
    SerializedProperty propMode;
    SerializedProperty propTargetTag;
    SerializedProperty propTargetObject;
    SerializedProperty propFollowSpeed;
    SerializedProperty propRect;
    SerializedProperty propCameraLimits;

    protected void OnEnable()
    {
        propShowInfo = serializedObject.FindProperty("_showInfo");
        propDescription = serializedObject.FindProperty("_description");
        propMode = serializedObject.FindProperty("mode");
        propTargetTag = serializedObject.FindProperty("targetTag");
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

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            StdEditor(false);
        }
    }

    protected virtual bool WriteTitle()
    {
        CameraFollow2d cameraFollow = target as CameraFollow2d;
        if (cameraFollow == null) { return true; }

        GUIStyle styleTitle = GUIUtils.GetActionTitleStyle();
        GUIStyle explanationStyle = GUIUtils.GetActionExplanationStyle();

        var backgroundColor = GUIUtils.ColorFromHex("#ffcaca");
        var textColor = GUIUtils.ColorFromHex("#2f4858");
        var separatorColor = GUIUtils.ColorFromHex("#ff6060");

        string title = "Camera Follow";

        // Compute explanation text height
        string explanation = cameraFollow.UpdateDescription();
        int explanationLines = explanation.Count((c) => c == '\n') + 1;
        int explanationTextHeight = explanationLines * (explanationStyle.fontSize + 2) + 6;

        var varTexture = GUIUtils.GetTexture("MovementTexture");
        if (varTexture == null)
        {
            varTexture = GUIUtils.AddTexture("MovementTexture", new CodeBitmaps.Movement());
        }

        // Background and title
        float inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
        Rect titleRect = EditorGUILayout.BeginVertical("box");
        Rect rect = new Rect(titleRect.x, titleRect.y, inspectorWidth - titleRect.x, styleTitle.fontSize + 16);
        Rect fullRect = rect;
        if (explanation != "")
        {
            fullRect.height = rect.height + 8 + explanationTextHeight;
        }
        EditorGUI.DrawRect(fullRect, backgroundColor);
        var prevColor = styleTitle.normal.textColor;
        styleTitle.normal.textColor = textColor;
        GUI.DrawTexture(new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32), varTexture, ScaleMode.ScaleToFit, true, 1.0f);
        EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 8, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), title, styleTitle);
        styleTitle.normal.textColor = prevColor;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(fullRect.height);

        if (explanation != "")
        {
            // Separator
            Rect separatorRect = new Rect(titleRect.x + 4, titleRect.y + rect.height, inspectorWidth - 20 - 8, 4);
            EditorGUI.DrawRect(separatorRect, separatorColor);

            // Explanation
            EditorGUI.LabelField(new Rect(titleRect.x + 10, separatorRect.y + separatorRect.height + 4, inspectorWidth - 20 - titleRect.x - 4, explanationTextHeight), explanation, explanationStyle);
        }


        bool toggle = false;
        if (propShowInfo.boolValue)
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeClose", BuildEyeClose));
        }
        else
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeOpen", BuildEyeOpen));
        }
        if (toggle)
        {
            propShowInfo.boolValue = !propShowInfo.boolValue;

            serializedObject.ApplyModifiedProperties();
        }

        return propShowInfo.boolValue;
    }


    protected void StdEditor(bool useOriginalEditor = true)
    {
        // Draw old editor, need it for now
        if (useOriginalEditor)
        {
            base.OnInspectorGUI();
        }

    }

    private void BuildEyeOpen(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeOpen());
    }

    private void BuildEyeClose(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeClose());
    }

    private void BuildTitleButton(string name, GUIBitmap bitmap)
    {
        Color iconColor = GUIUtils.ColorFromHex("#2f4858");
        Color borderColor = GUIUtils.ColorFromHex("#2f4858");
        Color normalBackColor = GUIUtils.ColorFromHex("#a8b591");
        Color hoverBackColor = GUIUtils.ColorFromHex("#cbdbaf");

        var bitmap_normal = new GUIBitmap(bitmap);
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_normal.FillAlpha(normalBackColor);
        GUIUtils.BitmapToTexture($"{name}:normal", bitmap_normal);

        var bitmap_highlight = new GUIBitmap(bitmap);
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_highlight.FillAlpha(hoverBackColor);
        GUIUtils.BitmapToTexture($"{name}:hover", bitmap_highlight);
    }

}
