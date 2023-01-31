using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using System.Linq;

[CustomEditor(typeof(Trigger))]
public class TriggerEditor : Editor
{
    SerializedProperty propShowInfo;
    SerializedProperty propExplanation;
    SerializedProperty propEnableTrigger;
    SerializedProperty propAllowRetrigger;
    SerializedProperty propHasConditions;
    SerializedProperty propConditions;
    SerializedProperty propActions;

    protected virtual void OnEnable()
    {
        propShowInfo = serializedObject.FindProperty("_showInfo");
        propExplanation = serializedObject.FindProperty("_explanation");
        propEnableTrigger = serializedObject.FindProperty("enableTrigger");
        propAllowRetrigger = serializedObject.FindProperty("allowRetrigger");
        propHasConditions = serializedObject.FindProperty("hasPreconditions");
        propConditions = serializedObject.FindProperty("preConditions");
        propActions = serializedObject.FindProperty("actions");
    }

    public override void OnInspectorGUI()
    {
        if (WriteTitle())
        {
            StdEditor();
        }
    }

    public virtual Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("TriggerTexture");
        if (varTexture == null)
        {
            varTexture = GUIUtils.AddTexture("TriggerTexture", new CodeBitmaps.Trigger());
        }

        return varTexture;
    }


    protected virtual bool WriteTitle()
    {
        Trigger trigger = target as Trigger;
        if (trigger == null) { return true; }

        GUIStyle styleTitle = GUIUtils.GetTriggerTitleStyle();
        GUIStyle explanationStyle = GUIUtils.GetTriggerExplanationStyle();

        var backgroundColor = GUIUtils.ColorFromHex("#D0FFFF");
        var textColor = GUIUtils.ColorFromHex("#2f4858");
        var separatorColor = GUIUtils.ColorFromHex("#86CBFF");

        // Compute explanation text height
        string explanation = propExplanation.stringValue;
        int explanationLines = explanation.Count((c) => c == '\n') + 1;
        int explanationTextHeight = explanationLines * (explanationStyle.fontSize + 2) + 6;

        // Background and title
        float inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
        Rect titleRect = EditorGUILayout.BeginVertical("box");
        Rect rect = new Rect(titleRect.x, titleRect.y, inspectorWidth - titleRect.x, styleTitle.fontSize + 14);
        Rect fullRect = rect;
        if (explanation != "")
        {
            fullRect.height = rect.height + 6 + explanationTextHeight;
        }
        EditorGUI.DrawRect(fullRect, backgroundColor);
        var prevColor = styleTitle.normal.textColor;
        styleTitle.normal.textColor = textColor;
        GUI.DrawTexture(new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32), GetIcon(), ScaleMode.ScaleToFit, true, 1.0f);
        EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 6, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), trigger.GetTriggerTitle(), styleTitle);
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
        if (trigger.showInfo)
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
                // Affect all the triggers in this object
                var allTriggers = trigger.GetComponents<Trigger>();
                foreach (var t in allTriggers)
                {
                    t.showInfo = propShowInfo.boolValue;
                    t.UpdateExplanation();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        if (refreshExplanation)
        {
            propExplanation.stringValue = trigger.GetDescription();
            serializedObject.ApplyModifiedProperties();
        }

        return propShowInfo.boolValue;
    }


    protected void StdEditor(bool useOriginalEditor = true)
    {
        Rect rect = EditorGUILayout.BeginHorizontal();
        rect.height = 20;
        float totalWidth = rect.width;
        float elemWidth = totalWidth / 3;
        propEnableTrigger.boolValue = CheckBox("Active", rect.x, rect.y, elemWidth, propEnableTrigger.boolValue);
        propAllowRetrigger.boolValue = CheckBox("Allow Retrigger", rect.x + elemWidth, rect.y, elemWidth, propAllowRetrigger.boolValue);
        propHasConditions.boolValue = CheckBox("Conditions", rect.x + elemWidth * 2, rect.y, elemWidth, propHasConditions.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(rect.height);

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

    protected void ActionPanel()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(propActions, new GUIContent("Actions"), true);
        serializedObject.ApplyModifiedProperties();
    }

    private bool CheckBox(string label, float x, float y, float width, bool initialValue)
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
