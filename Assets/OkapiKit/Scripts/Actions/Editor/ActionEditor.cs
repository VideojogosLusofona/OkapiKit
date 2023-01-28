using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Rendering.DebugUI.MessageBox;

/*
[CustomEditor(typeof(Action))]
public class ActionEditor : Editor
{
    SerializedProperty propShowInfo;

    void OnEnable()
    {
        propShowInfo = serializedObject.FindProperty("_showInfo");
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

        Rect rect = GUIUtils.DrawOutlineLabel(action.GetActionTitle(), styleTitle, GUIUtils.ColorFromHex("#86CB92"), GUIUtils.ColorFromHex("#D7E8BA"), GUIUtils.ColorFromHex("#2f4858"));

        bool toggle = false;
        if (action.showInfo)
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 26, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeClose", BuildEyeClose));
        }
        else
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 26, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeOpen", BuildEyeOpen));
        }
        if (toggle)
        {
            propShowInfo.boolValue = !propShowInfo.boolValue;

            Event e = Event.current;
            if (e.shift)
            {
                // Affect all the Actions in this object
                var allActions = action.GetComponents<Action>();
                foreach (var a in allActions)
                {
                    a.showInfo = propShowInfo.boolValue;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        return propShowInfo.boolValue;
    }


    protected void StdEditor()
    {
        base.OnInspectorGUI();
    }


    private void BuildEyeOpen(string name)
    {
        Color iconColor = GUIUtils.ColorFromHex("#2f4858");
        Color borderColor = GUIUtils.ColorFromHex("#2f4858");
        Color normalBackColor = GUIUtils.ColorFromHex("#a8b591");
        Color hoverBackColor = GUIUtils.ColorFromHex("#cbdbaf");

        var bitmap_normal = new CodeBitmaps.EyeOpen();
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_normal.FillAlpha(normalBackColor);
        GUIUtils.BitmapToTexture($"{name}:normal", bitmap_normal);

        var bitmap_highlight = new CodeBitmaps.EyeOpen();
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_highlight.FillAlpha(hoverBackColor);
        GUIUtils.BitmapToTexture($"{name}:hover", bitmap_highlight);
    }

    private void BuildEyeClose(string name)
    {
        Color iconColor = GUIUtils.ColorFromHex("#2f4858");
        Color borderColor = GUIUtils.ColorFromHex("#2f4858");
        Color normalBackColor = GUIUtils.ColorFromHex("#a8b591");
        Color hoverBackColor = GUIUtils.ColorFromHex("#cbdbaf");

        var bitmap_normal = new CodeBitmaps.EyeClose();
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_normal.FillAlpha(normalBackColor);
        GUIUtils.BitmapToTexture($"{name}:normal", bitmap_normal);

        var bitmap_highlight = new CodeBitmaps.EyeClose();
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_highlight.FillAlpha(hoverBackColor);
        GUIUtils.BitmapToTexture($"{name}:hover", bitmap_highlight);
    }
}
*/