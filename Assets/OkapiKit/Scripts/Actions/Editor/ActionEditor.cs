using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Action))]
public class ActionEditor : OkapiBaseEditor
{
    SerializedProperty propEnableAction;
    SerializedProperty propHasTags;
    SerializedProperty propHasConditions;
    SerializedProperty propTags;
    SerializedProperty propConditions;

    protected override void OnEnable()
    {
        base.OnEnable();
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

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Action");

        return varTexture;
    }

    protected override (Color, Color, Color) GetColors()
    {
        if (propEnableAction.boolValue)
        {
            return (GUIUtils.ColorFromHex("#D7E8BA"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#86CB92"));
        }
        else
        {
            return (GUIUtils.ColorFromHex("#94ad69"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#3e894b"));
        }
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
        return (target as Action).GetActionTitle();
    }
}
