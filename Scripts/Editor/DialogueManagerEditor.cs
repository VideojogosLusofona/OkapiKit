using UnityEngine;
using UnityEditor;
using OkapiKit;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(DialogueManager))]
    public class DialogueManagerEditor : OkapiBaseEditor
    {
        SerializedProperty dialogueDataProp;
        SerializedProperty displayProp;

        DialogueManager dialogueManager;

        protected override void OnEnable()
        {
            base.OnEnable();

            dialogueManager = (DialogueManager)target;

            dialogueDataProp = serializedObject.FindProperty("dialogueData");
            displayProp = serializedObject.FindProperty("display");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Title and Explanation
            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(dialogueDataProp, new GUIContent("Dialogue Data", "All the data that can be used in the dialogues."));
                EditorGUILayout.PropertyField(displayProp, new GUIContent("Dialogue Display", "The UI element used to display the dialogue."));

                serializedObject.ApplyModifiedProperties();
            }

            (target as OkapiElement)?.UpdateExplanation();
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle()
        {
            return "Dialogue Manager";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Talk"); // Replace with actual texture for the DialogueManager
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
