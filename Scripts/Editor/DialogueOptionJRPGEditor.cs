using UnityEngine;
using UnityEditor;
using OkapiKit;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(DialogueOptionJRPG))]
    public class DialogueOptionJRPGEditor : OkapiBaseEditor
    {
        SerializedProperty optionTextProp;
        SerializedProperty optionTextNormalColorProp;
        SerializedProperty optionTextSelectedColorProp;
        SerializedProperty selectorBarSelectedProp;
        SerializedProperty optionBarNormalColorProp;
        SerializedProperty optionBarSelectedColorProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            optionTextProp = serializedObject.FindProperty("optionText");
            optionTextNormalColorProp = serializedObject.FindProperty("optionTextNormalColor");
            optionTextSelectedColorProp = serializedObject.FindProperty("optionTextSelectedColor");
            selectorBarSelectedProp = serializedObject.FindProperty("selectorBarSelected");
            optionBarNormalColorProp = serializedObject.FindProperty("optionBarNormalColor");
            optionBarSelectedColorProp = serializedObject.FindProperty("optionBarSelectedColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Title and Explanation
            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(optionTextProp, new GUIContent("Option Text", "The text element for displaying the text for this option."));
                EditorGUILayout.PropertyField(optionTextNormalColorProp, new GUIContent("Normal Text Color", "The color of the text when the option is not selected."));
                EditorGUILayout.PropertyField(optionTextSelectedColorProp, new GUIContent("Selected Text Color", "The color of the text when the option is selected."));
                EditorGUILayout.PropertyField(selectorBarSelectedProp, new GUIContent("Selector Bar", "The bar used to highlight the selected option."));
                EditorGUILayout.PropertyField(optionBarNormalColorProp, new GUIContent("Normal Bar Color", "The color of the bar when the option is not selected."));
                EditorGUILayout.PropertyField(optionBarSelectedColorProp, new GUIContent("Selected Bar Color", "The color of the bar when the option is selected."));

                serializedObject.ApplyModifiedProperties();
            }

            (target as OkapiElement)?.UpdateExplanation();
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle()
        {
            return "JRPG Dialogue Option";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Talk"); // Replace with actual texture for the DialogueOptionJRPG
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override (Color, Color, Color) GetColors()
        {
            return (
                GUIUtils.ColorFromHex("#C7D8D8"), GUIUtils.ColorFromHex("#4F4F8C"), GUIUtils.ColorFromHex("#8080D0")
            );
        }
    }
}
