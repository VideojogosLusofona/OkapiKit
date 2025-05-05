using UnityEngine;
using UnityEditor;
using OkapiKit;
using System.Collections.Generic;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : OkapiBaseEditor
    {
        SerializedProperty dialoguesProp;

        DialogueData dialogueData;

        protected override void OnEnable()
        {
            base.OnEnable();

            dialogueData = (DialogueData)target;

            dialoguesProp = serializedObject.FindProperty("dialogues");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Title and Explanation
            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(dialoguesProp, new GUIContent("Dialogues", "List of all dialogues in this data."));

                serializedObject.ApplyModifiedProperties();
            }

            (target as OkapiElement)?.UpdateExplanation();
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle()
        {
            return "Dialogue Data";
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Talk");
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
            return (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#000000"), GUIUtils.ColorFromHex("#969721"));
        }
    }
}
