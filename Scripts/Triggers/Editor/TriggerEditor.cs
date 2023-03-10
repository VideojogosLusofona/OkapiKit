using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Trigger))]
public class TriggerEditor : OkapiBaseEditor
{
    protected SerializedProperty propEnableTrigger;
    protected SerializedProperty propAllowRetrigger;
    protected SerializedProperty propHasConditions;
    protected SerializedProperty propPreConditions;
    protected SerializedProperty propActions;

    protected override void OnEnable()
    {
        base.OnEnable();

        propEnableTrigger = serializedObject.FindProperty("enableTrigger");
        propAllowRetrigger = serializedObject.FindProperty("allowRetrigger");
        propHasConditions = serializedObject.FindProperty("hasPreconditions");
        propPreConditions = serializedObject.FindProperty("preConditions");
        propActions = serializedObject.FindProperty("actions");
    }

    public override void OnInspectorGUI()
    {
        if (WriteTitle())
        {
            StdEditor();
        }
    }

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Trigger");

        return varTexture;
    }

    protected void StdEditor(bool useOriginalEditor = true, bool allowConditions = true)
    {
        Rect rect = EditorGUILayout.BeginHorizontal();
        rect.height = 20;
        float totalWidth = rect.width;
        float elemWidth = totalWidth / 3;
        propEnableTrigger.boolValue = CheckBox("Active", rect.x, rect.y, elemWidth, propEnableTrigger.boolValue);
        propAllowRetrigger.boolValue = CheckBox("Allow Retrigger", rect.x + elemWidth, rect.y, elemWidth, propAllowRetrigger.boolValue);
        if (allowConditions)
        {
            propHasConditions.boolValue = CheckBox("Conditions", rect.x + elemWidth * 2, rect.y, elemWidth, propHasConditions.boolValue);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(rect.height);
        EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

        if (propHasConditions.boolValue)
        {
            // Display tags
            EditorGUILayout.PropertyField(propPreConditions, new GUIContent("Conditions"), true);
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
        serializedObject.ApplyModifiedProperties();

        serializedObject.Update();
        EditorGUILayout.PropertyField(propActions, new GUIContent("Actions"), true);
        
        serializedObject.ApplyModifiedProperties();
        (target as Trigger).UpdateExplanation();
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

    protected override GUIStyle GetTitleSyle()
    {
        return GUIUtils.GetTriggerTitleStyle();
    }

    protected override GUIStyle GetExplanationStyle()
    {
        return GUIUtils.GetTriggerExplanationStyle();
    }

    protected override string GetTitle()
    {
        return (target as Trigger).GetTriggerTitle();
    }

    protected override (Color, Color, Color) GetColors()
    {
        if (propEnableTrigger.boolValue)
        {
            return (GUIUtils.ColorFromHex("#D0FFFF"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#86CBFF"));
        }
        else
        {
            return (GUIUtils.ColorFromHex("#80c5c5"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#4e7694"));
        }
    }
}
