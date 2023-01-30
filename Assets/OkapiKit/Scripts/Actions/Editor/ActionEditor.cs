using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using System.Linq;

[CustomEditor(typeof(Action))]
public class ActionEditor : Editor
{
    SerializedProperty propShowInfo;
    SerializedProperty propExplanation;
    SerializedProperty propEnableAction;
    SerializedProperty propHasTags;
    SerializedProperty propHasConditions;
    SerializedProperty propTags;
    SerializedProperty propConditions;

    protected virtual void OnEnable()
    {
        propShowInfo = serializedObject.FindProperty("_showInfo");
        propExplanation = serializedObject.FindProperty("_explanation");
        propEnableAction = serializedObject.FindProperty("enableAction");
        propHasTags = serializedObject.FindProperty("hasTags");
        propHasConditions = serializedObject.FindProperty("hasConditions");
        propTags = serializedObject.FindProperty("actionTags");
        propConditions = serializedObject.FindProperty("actionConditions");
    }

    public override void OnInspectorGUI()
    {
        if (WriteTitle())
        {
            StdEditor();
        }
    }

    protected virtual bool WriteTitle()
    {
        Action action = target as Action;
        if (action == null) { return true; }

        GUIStyle styleTitle = GUIUtils.GetActionTitleStyle();
        GUIStyle explanationStyle = GUIUtils.GetActionExplanationStyle();

        var backgroundColor = GUIUtils.ColorFromHex("#D7E8BA");
        var textColor = GUIUtils.ColorFromHex("#2f4858");
        var separatorColor = GUIUtils.ColorFromHex("#86CB92");

        // Compute explanation text height
        string explanation = propExplanation.stringValue;
        int explanationLines = explanation.Count((c) => c == '\n');
        explanationLines += 1;
        int explanationTextHeight = explanationLines * explanationStyle.fontSize + 6;

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
        EditorGUI.LabelField(new Rect(titleRect.x + 10, titleRect.y + 6, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), action.GetActionTitle(), styleTitle);
        styleTitle.normal.textColor = prevColor;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(fullRect.height);

        if (explanation != "")
        {
            // Separator
            Rect separatorRect = new Rect(titleRect.x + 4, titleRect.y + rect.height, inspectorWidth - 20 - 8, 4);
            EditorGUI.DrawRect(separatorRect, separatorColor);

            // Explanation
            EditorGUI.LabelField(new Rect(titleRect.x + 10, separatorRect.y + separatorRect.height + 4 , inspectorWidth - 20 - titleRect.x - 4, explanationTextHeight), explanation, explanationStyle);
        }


        bool toggle = false;
        bool refreshExplanation = false;
        if (action.showInfo)
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeClose", BuildEyeClose));
        }
        else
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeOpen", BuildEyeOpen));
        }
        refreshExplanation = GUI.Button(new Rect(rect.x + rect.width - 26, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("Refresh", BuildRefresh));
        if (toggle)
        {
            refreshExplanation = true;
            propShowInfo.boolValue = !propShowInfo.boolValue;

            Event e = Event.current;
            if (e.shift)
            {
                // Affect all the Actions in this object
                var allActions = action.GetComponents<Action>();
                foreach (var a in allActions)
                {
                    a.showInfo = propShowInfo.boolValue;
                    a.UpdateExplanation();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        if (refreshExplanation)
        {
            action.UpdateExplanation();
        }

        return propShowInfo.boolValue;
    }


    protected void StdEditor(bool useOriginalEditor = true)
    {
        Rect rect = EditorGUILayout.BeginHorizontal();
        rect.height = 20;
        float totalWidth = rect.width;
        float elemWidth = totalWidth / 3;
        propEnableAction.boolValue = CheckBox("Active", rect.x, rect.y, elemWidth, propEnableAction.boolValue);
        propHasTags.boolValue = CheckBox("Tags", rect.x + elemWidth, rect.y, elemWidth, propHasTags.boolValue);
        propHasConditions.boolValue = CheckBox("Conditions", rect.x + elemWidth * 2, rect.y, elemWidth, propHasConditions.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(rect.height);

        if (propHasTags.boolValue)
        {
            // Display tags
            EditorGUILayout.PropertyField(propTags, new GUIContent("Tags"), true);
        }

        if (propHasConditions.boolValue)
        {
            // Display tags
            EditorGUILayout.PropertyField(propConditions, new GUIContent("Conditions"), true);
        }

        serializedObject.ApplyModifiedProperties();

        // Draw old editor, need it for now
        if (useOriginalEditor)
        {
            base.OnInspectorGUI();
        }

    }

    protected bool CheckBox(string label, float x, float y, float width, bool initialValue)
    {
        GUIStyle style = GUI.skin.toggle;
        Vector2  size = style.CalcSize(new GUIContent(label));

        EditorGUI.LabelField(new Rect(x, y, size.x, 20), label);
        float offsetX = size.x + 1;

        if (offsetX + 20 > width) offsetX = width - 20;

        bool ret = EditorGUI.Toggle(new Rect(x + offsetX, y, 20, 20), initialValue);

        return ret;
    }

    private void BuildEyeOpen(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeOpen());
    }

    private void BuildEyeClose(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeClose());
    }

    private void BuildRefresh(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.Refresh());
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
