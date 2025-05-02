using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : OkapiBaseEditor
    {
        SerializedProperty displayNameProp;
        SerializedProperty questTextProp;
        SerializedProperty questsRequired;
        SerializedProperty questObjectives;

        Quest quest;

        protected override void OnEnable()
        {
            base.OnEnable();

            quest = (Quest)target;

            displayNameProp = serializedObject.FindProperty("displayName");
            questTextProp = serializedObject.FindProperty("questText");
            questsRequired = serializedObject.FindProperty("questsRequired");
            questObjectives = serializedObject.FindProperty("questObjectives");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(displayNameProp, new GUIContent("Display Name", "Display name of the quest"));
                EditorGUILayout.PropertyField(questTextProp, new GUIContent("Quest Text", "Quest text of the quest"));

                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(questsRequired, new GUIContent("Required Quests", "These quests need to be done before this quest can be active"), true);
                EditorGUILayout.PropertyField(questObjectives, new GUIContent("Quest objectives", "Quest objectives"), true);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Quest).UpdateExplanation();
            }
        }

        protected override string GetTitle()
        {
            return !string.IsNullOrEmpty(quest.displayName) ? quest.displayName : quest.name;
        }

        protected override Texture2D GetIcon()
        {
            return GUIUtils.GetTexture("Quest");
        }

        protected override (Color, Color, Color) GetColors()
        {
            return (GUIUtils.ColorFromHex("#FFE7A1"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#d9b957"));
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }
    }
}
